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

    public async Task<Result<GaveteroDetailDto>> Create(GaveteroEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<GaveteroDetailDto>.Error("Error al crear gavetero")
            : Result<GaveteroDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<GaveteroDetailDto>> Update(GaveteroEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<GaveteroDetailDto>.Error("Error al actualizar gavetero")
            : Result<GaveteroDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar gavetero");
    }

    public async Task<Result<GaveteroDetailDto>> Get(int id)
    {
        var gavetero = await _repository.Get(id);
        return !gavetero.IsSuccess
            ? Result<GaveteroDetailDto>.NotFound()
            : Result<GaveteroDetailDto>.Success(MapListDtoToDetailDto(gavetero.Value));
    }

    public async Task<Result<List<GaveteroListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<GaveteroListDto>>.Success(result.Value)
            : Result<List<GaveteroListDto>>.Error("Error al obtener gaveteros");
    }

    private static GaveteroDetailDto MapEntityToDetailDto(GaveteroEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        IdMueble = entity.IdMueble,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static GaveteroDetailDto MapListDtoToDetailDto(GaveteroListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        IdMueble = dto.IdMueble,
        EstadoEliminado = false
    };
}
