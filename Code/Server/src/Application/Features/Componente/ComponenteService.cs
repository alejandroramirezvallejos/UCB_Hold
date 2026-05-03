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

    public async Task<Result<ComponenteDetail>> Create(ComponenteEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<ComponenteDetail>.Error("Error al crear componente")
            : Result<ComponenteDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<ComponenteDetail>> Update(ComponenteEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<ComponenteDetail>.Error("Error al actualizar componente")
            : Result<ComponenteDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar componente");
    }

    public async Task<Result<ComponenteDetail>> Get(int id)
    {
        var componente = await _repository.Get(id);
        
        return !componente.IsSuccess
            ? Result<ComponenteDetail>.NotFound()
            : Result<ComponenteDetail>.Success(MapListToDetail(componente.Value));
    }

    public async Task<Result<List<ComponenteList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<ComponenteList>>.Success(result.Value)
            : Result<List<ComponenteList>>.Error("Error al obtener componentes");
    }

    private static ComponenteDetail MapListToDetail(ComponenteList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Modelo = dto.Modelo,
        Descripcion = dto.Descripcion,
        PrecioReferencia = dto.Precio,
        EstadoEliminado = false
    };
}
