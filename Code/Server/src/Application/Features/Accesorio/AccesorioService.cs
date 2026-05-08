using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService : Service<AccesorioEntity, AccesorioRepository, AccesorioDto>
{
    private readonly AccesorioRepository _repository;

    public AccesorioService(AccesorioRepository repository)
        : base(repository)
    {
        _repository = repository;
    }

    public override async Task<Result<AccesorioDto>> Create(AccesorioEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<AccesorioDto>.Error("Equipo no encontrado");

        return await base.Create(entity);
    }

    public override async Task<Result<AccesorioDto>> Update(AccesorioEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<AccesorioDto>.Error("Equipo no encontrado");

        return await base.Update(entity);
    }

    public async Task<Result<AccesorioDto>> CreateFromDto(AccesorioDto dto)
    {
        var equipoId = dto.IdEquipo ?? 0;
        if (equipoId <= 0 && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado) && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImtInt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;

        var entity = new AccesorioEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            UrlDataSheet = dto.UrlDataSheet,
            IdEquipo = equipoId
        };
        return await Create(entity);
    }

    public async Task<Result<AccesorioDto>> UpdateFromDto(int id, AccesorioDto dto)
    {
        var equipoId = dto.IdEquipo ?? 0;
        if (equipoId <= 0 && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado) && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImtInt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;

        var entity = new AccesorioEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            UrlDataSheet = dto.UrlDataSheet,
            IdEquipo = equipoId
        };
        return await Update(entity);
    }
}
