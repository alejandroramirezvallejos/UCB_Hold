using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;

namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService
{
    private readonly ComponenteRepository _repository;

    public ComponenteService(ComponenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ComponenteDetailDto>> Create(ComponenteEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<ComponenteDetailDto>.Error("Error al crear componente")
            : Result<ComponenteDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<ComponenteDetailDto>> Update(ComponenteEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<ComponenteDetailDto>.Error("Error al actualizar componente")
            : Result<ComponenteDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null)
            : Result<object>.Error("Error al eliminar componente");
    }

    public async Task<Result<ComponenteDetailDto>> Get(int id)
    {
        var componente = await _repository.Get(id);
        return !componente.IsSuccess
            ? Result<ComponenteDetailDto>.NotFound()
            : Result<ComponenteDetailDto>.Success(MapListDtoToDetailDto(componente.Value));
    }

    public async Task<Result<List<ComponenteListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<ComponenteListDto>>.Success(result.Value)
            : Result<List<ComponenteListDto>>.Error("Error al obtener componentes");
    }

    private static ComponenteDetailDto MapEntityToDetailDto(ComponenteEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Descripcion = entity.Descripcion,
        Modelo = entity.Modelo,
        Precio = entity.PrecioReferencia.HasValue ? (decimal)entity.PrecioReferencia.Value : null,
        IdEquipo = entity.IdEquipo,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static ComponenteDetailDto MapListDtoToDetailDto(ComponenteListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Descripcion = dto.Descripcion,
        Modelo = dto.Modelo,
        Precio = dto.Precio,
        IdEquipo = dto.IdEquipo,
        EstadoEliminado = false
    };
}
