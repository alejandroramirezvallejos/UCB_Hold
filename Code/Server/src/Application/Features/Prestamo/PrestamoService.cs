using System.Text.Json;
using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Application.Features.Prestamo.State;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly PrestamoMapper _mapper;

    public PrestamoService(PrestamoRepository repository, PrestamoMapper mapper,
        IValidator<PrestamoDto> validator, AuditLogService audit)
        : base(repository, validator, mapper, audit) =>
        _mapper = mapper;

    public override async Task<Result<PrestamoDto>> Create(PrestamoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<PrestamoDto>();

        var entity = MapToEntity(dto);

        if (await Repository.HasAtrasadoPrestamo(entity.Carnet!))
            return Result<PrestamoDto>.Error("Tiene un préstamo con devolución atrasada. Devuelva los equipos antes de realizar una nueva reserva.");

        entity.FechaSolicitud = dto.FechaSolicitud ?? DateTime.UtcNow;
        entity.FechaPrestamo = dto.FechaPrestamo ?? dto.FechaPrestamoEsperada;
        entity.FechaPrestamoEsperada = dto.FechaPrestamoEsperada ?? DateTime.UtcNow;
        entity.FechaDevolucionEsperada = dto.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7);
        entity.EstadoPrestamo = EstadoPrestamo.Pendiente;

        foreach (var grupoId in dto.GrupoEquipoId ?? [])
        {
            var disponible = await Repository.HasAvailableEquipo(
                grupoId, entity.FechaPrestamoEsperada, entity.FechaDevolucionEsperada);

            if (!disponible)
            {
                var nombre = await Repository.GetGrupoEquipoNombre(grupoId);
                
                return Result<PrestamoDto>.Error(
                    $"'{nombre ?? grupoId.ToString()}' no tiene unidades disponibles en las fechas seleccionadas");
            }
        }

        await Repository.SavePrestamo(entity);
        await Repository.SaveGrupoReservas(entity.Id, dto.GrupoEquipoId);
        await Repository.SaveContrato(entity, dto.Contrato);
        await Audit!.Log(AuditAccion.Crear, typeof(PrestamoEntity).Name, entity.Id.ToString());

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

    public async Task<Result<PrestamoDto>> UpdateStatus(int id, string newStatus, string? observacion = null, PrestamoDto? body = null)
    {
        var prestamo = await Repository.FindById(id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var parsedState = PrestamoState.Parse(newStatus);

        if (!parsedState.HasValue)
            return Result<PrestamoDto>.Error($"Estado '{newStatus}' no reconocido");

        if (!PrestamoState.CanTransition(prestamo.EstadoPrestamo, parsedState.Value))
            return Result<PrestamoDto>.Error($"Transición '{PrestamoState.ToText(prestamo.EstadoPrestamo)}' → '{newStatus}' no permitida");

        if (parsedState.Value == EstadoPrestamo.Aprobado)
        {
            var assigned = await Repository.AssignEquiposOnApproval(id);

            if (!assigned)
                return Result<PrestamoDto>.Error("No se puede aprobar: no hay equipos disponibles para uno o más grupos en las fechas solicitadas");

            await Repository.UpdateContratoWithEquipos(id);
        }

        prestamo.EstadoPrestamo = parsedState.Value;

        if (observacion != null)
            prestamo.Observacion = observacion;

        if (parsedState.Value == EstadoPrestamo.Finalizado)
            prestamo.FechaDevolucion = DateTime.UtcNow;

        await Repository.UpdateTracked(prestamo);

        string? detalleAudit = null;
        
        if (parsedState.Value == EstadoPrestamo.Finalizado && body?.EquiposRetorno is { Count: > 0 })
        {
            var estadosPorCodigo = new Dictionary<int, EstadoEquipo>();

            foreach (var item in body.EquiposRetorno)
            {
                if (int.TryParse(item.CodigoImt, out var codigo))
                    estadosPorCodigo[codigo] = ParseEstadoEquipo(item.EstadoEquipo);
            }

            var aplicados = await Repository.AplicarEstadosRetorno(id, estadosPorCodigo);

            detalleAudit = JsonSerializer.Serialize(new
            {
                observacion,
                equipos = aplicados.Select(a => new { codigo = a.Codigo, nombre = a.Nombre, estado = a.Estado })
            });
        }

        var accionAudit = parsedState.Value switch
        {
            EstadoPrestamo.Aprobado   => AuditAccion.Aprobar,
            EstadoPrestamo.Rechazado  => AuditAccion.Rechazar,
            EstadoPrestamo.Activo     => AuditAccion.Recoger,
            EstadoPrestamo.Finalizado => AuditAccion.Devolver,
            EstadoPrestamo.Cancelado  => AuditAccion.Cancelar,
            EstadoPrestamo.Atrasado   => AuditAccion.AtrasadoAutomatico,
            _                         => AuditAccion.Editar
        };

        await Audit!.Log(accionAudit, typeof(PrestamoEntity).Name, id.ToString(), detalleAudit);

        return await Get(id);
    }

    private static EstadoEquipo ParseEstadoEquipo(string? estado) => estado switch
    {
        "parcialmente_operativo" => EstadoEquipo.ParcialmenteOperativo,
        "inoperativo"            => EstadoEquipo.Inoperativo,
        _                        => EstadoEquipo.Operativo
    };

    public async Task<Result<List<PrestamoDto>>> GetHistory(string carnetUsuario, string estadoPrestamo)
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
