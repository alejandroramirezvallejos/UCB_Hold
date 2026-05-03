using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService
{
    private readonly GaveteroRepository _repository;

    public GaveteroService(GaveteroRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GaveteroDetail>> Create(GaveteroEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<GaveteroDetail>.Error("Error al crear gavetero")
            : Result<GaveteroDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<GaveteroDetail>> Update(GaveteroEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<GaveteroDetail>.Error("Error al actualizar gavetero")
            : Result<GaveteroDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar gavetero");
    }

    public async Task<Result<GaveteroDetail>> Get(int id)
    {
        var gavetero = await _repository.Get(id);
        
        return !gavetero.IsSuccess
            ? Result<GaveteroDetail>.NotFound()
            : Result<GaveteroDetail>.Success(MapListToDetail(gavetero.Value));
    }

    public async Task<Result<List<GaveteroList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<GaveteroList>>.Success(result.Value)
            : Result<List<GaveteroList>>.Error("Error al obtener gaveteros");
    }

    private static GaveteroDetail MapListToDetail(GaveteroList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        IdMueble = dto.IdMueble,
        EstadoEliminado = false
    };
}
