using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Prestamo.State;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly PrestamoMapper _mapper;

    public PrestamoService(PrestamoRepository repository, PrestamoMapper mapper, IValidator<PrestamoDto> validator)
        : base(repository, validator, mapper) =>
        _mapper = mapper;

    public override async Task<Result<PrestamoDto>> Create(PrestamoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<PrestamoDto>();

        var entity = MapToEntity(dto);
        entity.FechaSolicitud = dto.FechaSolicitud ?? DateTime.UtcNow;
        entity.FechaPrestamo = dto.FechaPrestamo ?? dto.FechaPrestamoEsperada;
        entity.FechaPrestamoEsperada = dto.FechaPrestamoEsperada ?? DateTime.UtcNow;
        entity.FechaDevolucionEsperada = dto.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7);
        entity.EstadoPrestamo = EstadoPrestamo.Pendiente;

        await Repository.SavePrestamo(entity);
        await Repository.AssignEquipos(entity.Id, dto.GrupoEquipoId);
        await Repository.SaveContrato(entity, dto.Contrato);

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

    public async Task<Result<PrestamoDto>> UpdateStatus(int id, string newStatus)
    {
        var prestamo = await Repository.FindById(id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var parsedState = PrestamoState.Parse(newStatus);

        if (!parsedState.HasValue)
            return Result<PrestamoDto>.Error($"Estado '{newStatus}' no reconocido");

        if (!PrestamoState.CanTransition(prestamo.EstadoPrestamo, parsedState.Value))
            return Result<PrestamoDto>.Error($"Transición '{PrestamoState.ToText(prestamo.EstadoPrestamo)}' → '{newStatus}' no permitida");

        prestamo.EstadoPrestamo = parsedState.Value;
        await Repository.UpdateTracked(prestamo);

        return await Get(id);
    }

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
