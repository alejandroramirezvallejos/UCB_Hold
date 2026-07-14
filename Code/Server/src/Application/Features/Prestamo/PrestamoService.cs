using System.Text.Json;
using System.Globalization;
using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Application.Features.Notificacion;
using IMT_Reservas.Server.Application.Features.Prestamo.State;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;

namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly PrestamoMapper _mapper;
    private readonly NotificacionService _notifications;

    public PrestamoService(
        PrestamoRepository repository,
        PrestamoMapper mapper,
        IValidator<PrestamoDto> validator,
        AuditLogService audit,
        NotificacionService notifications
    )
        : base(repository, validator, mapper, audit)
    {
        _mapper = mapper;
        _notifications = notifications;
    }

    public override async Task<Result<PrestamoDto>> Create(PrestamoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<PrestamoDto>();

        var entity = MapToEntity(dto);

        var eligibility = await EvaluateReservation(entity.Carnet!);

        if (!eligibility.PuedeReservar)
            return Result<PrestamoDto>.Error(eligibility.Motivo!);

        entity.FechaSolicitud = dto.FechaSolicitud ?? DateTime.UtcNow;
        entity.FechaPrestamo = dto.FechaPrestamo ?? dto.FechaPrestamoEsperada;
        entity.FechaPrestamoEsperada = dto.FechaPrestamoEsperada ?? DateTime.UtcNow;
        entity.FechaDevolucionEsperada = dto.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7);
        entity.EstadoPrestamo = EstadoPrestamo.Pendiente;

        foreach (var grupoEquipoId in dto.GrupoEquipoId ?? [])
        {
            var available = await Repository.HasAvailableEquipo(
                grupoEquipoId,
                entity.FechaPrestamoEsperada,
                entity.FechaDevolucionEsperada
            );

            if (!available)
            {
                var grupoEquipoNombre = await Repository.GetGrupoEquipoNombre(grupoEquipoId);

                return Result<PrestamoDto>.Error(
                    $"'{grupoEquipoNombre ?? grupoEquipoId.ToString(CultureInfo.InvariantCulture)}' no tiene unidades disponibles en las fechas seleccionadas"
                );
            }
        }

        await Repository.SavePrestamo(entity);
        await Repository.SaveGrupoEquipoReservations(entity.Id, dto.GrupoEquipoId);
        await Repository.SaveContrato(entity, dto.Contrato);
        await Audit!.Log(
            AuditAccion.Crear,
            typeof(PrestamoEntity).Name,
            entity.Id.ToString(CultureInfo.InvariantCulture)
        );
        await _notifications.CreateForAdmins(
            TipoNotificacion.AdminNuevoPrestamo,
            "Nueva reserva",
            $"El usuario {entity.Carnet} realizó una reserva."
        );

        return await Repository.Get(entity.Id);
    }

    public override async Task<Result<PrestamoDto>> Update(int id, PrestamoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<PrestamoDto>();

        var entity = await Repository.FindById(id);

        if (entity == null)
            return Result<PrestamoDto>.NotFound();

        _mapper.Update(dto, entity);
        await Repository.UpdateTracked(entity);

        return await Get(id);
    }

    public async Task<Result<PrestamoDto>> UpdateStatus(
        int id,
        string newStatus,
        string? observacion = null,
        PrestamoDto? body = null
    )
    {
        var loan = await Repository.FindById(id);

        if (loan == null)
            return Result<PrestamoDto>.NotFound();

        var parsedState = PrestamoState.Parse(newStatus);

        if (!parsedState.HasValue)
            return Result<PrestamoDto>.Error($"Estado '{newStatus}' no reconocido");

        if (!PrestamoState.CanTransition(loan.EstadoPrestamo, parsedState.Value))
            return Result<PrestamoDto>.Error(
                $"Transición '{PrestamoState.ToText(loan.EstadoPrestamo)}' → '{newStatus}' no permitida"
            );

        if (parsedState.Value == EstadoPrestamo.Aprobado)
        {
            var assigned = await Repository.AssignEquiposOnApproval(id);

            if (!assigned)
                return Result<PrestamoDto>.Error(
                    "No se puede aprobar: no hay equipos disponibles para uno o más grupos en las fechas solicitadas"
                );

            await Repository.UpdateContratoWithEquipos(id);
        }

        loan.EstadoPrestamo = parsedState.Value;

        if (observacion != null)
            loan.Observacion = observacion;

        if (parsedState.Value == EstadoPrestamo.Finalizado)
            loan.FechaDevolucion = DateTime.UtcNow;

        await Repository.UpdateTracked(loan);

        string? auditDetail = null;

        if (parsedState.Value == EstadoPrestamo.Finalizado)
        {
            auditDetail = await HandleFinalizadoEquiposRetorno(id, observacion, body);
        }

        var auditAction = parsedState.Value switch
        {
            EstadoPrestamo.Aprobado => AuditAccion.Aprobar,
            EstadoPrestamo.Rechazado => AuditAccion.Rechazar,
            EstadoPrestamo.Activo => AuditAccion.Recoger,
            EstadoPrestamo.Finalizado => AuditAccion.Devolver,
            EstadoPrestamo.Cancelado => AuditAccion.Cancelar,
            EstadoPrestamo.Atrasado => AuditAccion.AtrasadoAutomatico,
            _ => AuditAccion.Editar,
        };

        await Audit!.Log(
            auditAction,
            typeof(PrestamoEntity).Name,
            id.ToString(CultureInfo.InvariantCulture),
            auditDetail
        );

        var hasDamagedEquipment =
            parsedState.Value == EstadoPrestamo.Finalizado
            && body?.EquiposRetorno != null
            && body.EquiposRetorno.Any(e =>
                ParseEstadoEquipo(e.EstadoEquipo) != EstadoEquipo.Operativo
            );

        await NotifyStatusChange(
            loan.Carnet!,
            parsedState.Value,
            observacion,
            auditDetail,
            hasDamagedEquipment
        );

        return await Get(id);
    }

    private async Task NotifyStatusChange(
        string carnet,
        EstadoPrestamo estado,
        string? observacion,
        string? detalle,
        bool hasDamagedEquipment
    )
    {
        switch (estado)
        {
            case EstadoPrestamo.Aprobado:
                await _notifications.Create(
                    carnet,
                    TipoNotificacion.PrestamoAprobado,
                    "Préstamo aprobado",
                    "Tu solicitud de préstamo fue aprobada."
                );
                break;
            case EstadoPrestamo.Rechazado:
                await _notifications.Create(
                    carnet,
                    TipoNotificacion.PrestamoRechazado,
                    "Préstamo rechazado",
                    observacion ?? "Tu solicitud de préstamo fue rechazada."
                );
                break;
            case EstadoPrestamo.Finalizado when hasDamagedEquipment:
                await _notifications.Create(
                    carnet,
                    TipoNotificacion.EquipoObservacion,
                    "Estado de equipo actualizado",
                    observacion,
                    detalle
                );
                break;
        }
    }

    private async Task<string?> HandleFinalizadoEquiposRetorno(
        int id,
        string? observacion,
        PrestamoDto? body
    )
    {
        if (body?.EquiposRetorno == null || body.EquiposRetorno.Count == 0)
            return null;

        var statesByCodigoImt = new Dictionary<int, EstadoEquipo>();

        foreach (var item in body.EquiposRetorno)
        {
            if (int.TryParse(item.CodigoImt, out var codigoImt))
                statesByCodigoImt[codigoImt] = ParseEstadoEquipo(item.EstadoEquipo);
        }

        var appliedReturns = await Repository.ApplyEstadoEquipoRetorno(id, statesByCodigoImt);

        return JsonSerializer.Serialize(
            new
            {
                observacion,
                equipos = appliedReturns.Select(appliedReturn => new
                {
                    codigo = appliedReturn.CodigoImt,
                    nombre = appliedReturn.NombreGrupoEquipo,
                    estado = appliedReturn.EstadoEquipo,
                }),
            }
        );
    }

    private static EstadoEquipo ParseEstadoEquipo(string? estado) =>
        estado switch
        {
            "parcialmente_operativo" => EstadoEquipo.ParcialmenteOperativo,
            "inoperativo" => EstadoEquipo.Inoperativo,
            _ => EstadoEquipo.Operativo,
        };

    public async Task<Result<EstadoReservaDto>> GetReservationStatus(string carnet) =>
        Result<EstadoReservaDto>.Success(await EvaluateReservation(carnet));

    private async Task<EstadoReservaDto> EvaluateReservation(string carnet)
    {
        if (await Repository.HasAtrasadoPrestamo(carnet))
            return new EstadoReservaDto
            {
                PuedeReservar = false,
                Motivo =
                    "Tiene un préstamo con devolución atrasada. Devuelva los equipos antes de realizar una nueva reserva.",
            };

        if (await Repository.IsUserBlocked(carnet))
        {
            var blockReason = await Repository.GetBlockReason(carnet);

            return new EstadoReservaDto
            {
                PuedeReservar = false,
                Motivo = string.IsNullOrWhiteSpace(blockReason)
                    ? "Cuenta bloqueada para reservas."
                    : $"Cuenta bloqueada para reservas: {blockReason}",
            };
        }

        return new EstadoReservaDto { PuedeReservar = true };
    }

    public async Task<Result<List<PrestamoDto>>> GetHistory(
        string carnetUsuario,
        string estadoPrestamo
    )
    {
        if (string.IsNullOrEmpty(carnetUsuario))
            return Result<List<PrestamoDto>>.Error("Carnet requerido");

        EstadoPrestamo? estado = null;

        if (!string.IsNullOrEmpty(estadoPrestamo) && estadoPrestamo != "todos")
        {
            estado = PrestamoState.Parse(estadoPrestamo);

            if (!estado.HasValue)
                return Result<List<PrestamoDto>>.Error("Estado préstamo no válido");
        }

        return await Repository.GetHistoryWithDetails(carnetUsuario, estado);
    }
}
