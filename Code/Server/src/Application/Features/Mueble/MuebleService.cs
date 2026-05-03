using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService
{
    private readonly MuebleRepository _repository;

    public MuebleService(MuebleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MuebleDetail>> Create(MuebleEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<MuebleDetail>.Error("Error al crear mueble")
            : Result<MuebleDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<MuebleDetail>> Update(MuebleEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<MuebleDetail>.Error("Error al actualizar mueble")
            : Result<MuebleDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar mueble");
    }

    public async Task<Result<MuebleDetail>> Get(int id)
    {
        var mueble = await _repository.Get(id);
        
        return !mueble.IsSuccess
            ? Result<MuebleDetail>.NotFound()
            : Result<MuebleDetail>.Success(MapListToDetail(mueble.Value));
    }

    public async Task<Result<List<MuebleList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<MuebleList>>.Success(result.Value)
            : Result<List<MuebleList>>.Error("Error al obtener muebles");
    }
    
    private static MuebleDetail MapListToDetail(MuebleList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Ubicacion = dto.Ubicacion,
        EstadoEliminado = false
    };
}
