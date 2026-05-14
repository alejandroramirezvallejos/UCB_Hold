using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService : Service<ComponenteEntity, ComponenteRepository, ComponenteDto>
{
    private readonly ComponenteRepository _repository;

    public ComponenteService(ComponenteRepository repository)
        : base(repository) => _repository = repository;

    public override async Task<Result<ComponenteDto>> Create(ComponenteEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<ComponenteDto>.Error("Equipo no encontrado");

        return await base.Create(entity);
    }

    public override async Task<Result<ComponenteDto>> Update(ComponenteEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<ComponenteDto>.Error("Equipo no encontrado");

        return await base.Update(entity);
    }

    public async Task<Result<ComponenteDto>> CreateFromDto(ComponenteDto dto)
    {
        var equipoId = dto.IdEquipo ?? 0;
        
        if (equipoId <= 0 && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipo) &&
            int.TryParse(dto.CodigoImtEquipo, out var codigoImtInt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;

        var entity = new ComponenteEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            PrecioReferencia = dto.PrecioReferencia,
            IdEquipo = equipoId,
            UrlDataSheet = dto.UrlDataSheet
        };
        
        return await Create(entity);
    }

    public async Task<Result<ComponenteDto>> UpdateFromDto(int id, ComponenteDto dto)
    {
        var equipoId = dto.IdEquipo ?? 0;
        
        if (equipoId <= 0 && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipo) &&
            int.TryParse(dto.CodigoImtEquipo, out var codigoImtInt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;

        var entity = new ComponenteEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            PrecioReferencia = dto.PrecioReferencia,
            IdEquipo = equipoId,
            UrlDataSheet = dto.UrlDataSheet
        };
        
        return await Update(entity);
    }
}
