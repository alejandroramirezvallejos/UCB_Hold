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

    public async Task<Result<CarreraDetailDto>> Create(CarreraEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<CarreraDetailDto>.Error("Error al crear carrera")
            : Result<CarreraDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<CarreraDetailDto>> Update(CarreraEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<CarreraDetailDto>.Error("Error al actualizar carrera")
            : Result<CarreraDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar carrera");
    }

    public async Task<Result<CarreraDetailDto>> Get(int id)
    {
        var carrera = await _repository.Get(id);
        
        return !carrera.IsSuccess
            ? Result<CarreraDetailDto>.NotFound()
            : Result<CarreraDetailDto>.Success(MapListDtoToDetailDto(carrera.Value));
    }

    public async Task<Result<List<CarreraListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<CarreraListDto>>.Success(result.Value)
            : Result<List<CarreraListDto>>.Error("Error al obtener carreras");
    }

    private static CarreraDetailDto MapListDtoToDetailDto(CarreraListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        EstadoEliminado = false
    };
}
