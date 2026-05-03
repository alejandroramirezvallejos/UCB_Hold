using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService
{
    private readonly CarreraRepository _repository;

    public CarreraService(CarreraRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CarreraDetail>> Create(CarreraEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<CarreraDetail>.Error("Error al crear carrera")
            : Result<CarreraDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<CarreraDetail>> Update(CarreraEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<CarreraDetail>.Error("Error al actualizar carrera")
            : Result<CarreraDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar carrera");
    }

    public async Task<Result<CarreraDetail>> Get(int id)
    {
        var carrera = await _repository.Get(id);
        
        return !carrera.IsSuccess
            ? Result<CarreraDetail>.NotFound()
            : Result<CarreraDetail>.Success(MapListToDetail(carrera.Value));
    }

    public async Task<Result<List<CarreraList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<CarreraList>>.Success(result.Value)
            : Result<List<CarreraList>>.Error("Error al obtener carreras");
    }

    private static CarreraDetail MapListToDetail(CarreraList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        EstadoEliminado = false
    };
}
