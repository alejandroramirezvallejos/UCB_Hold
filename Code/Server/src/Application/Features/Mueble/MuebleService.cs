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

    public async Task<Result<MuebleDetailDto>> Create(MuebleEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<MuebleDetailDto>.Error("Error al crear mueble")
            : Result<MuebleDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<MuebleDetailDto>> Update(MuebleEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<MuebleDetailDto>.Error("Error al actualizar mueble")
            : Result<MuebleDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null)
            : Result<object>.Error("Error al eliminar mueble");
    }

    public async Task<Result<MuebleDetailDto>> Get(int id)
    {
        var mueble = await _repository.Get(id);
        return !mueble.IsSuccess
            ? Result<MuebleDetailDto>.NotFound()
            : Result<MuebleDetailDto>.Success(MapListDtoToDetailDto(mueble.Value));
    }

    public async Task<Result<List<MuebleListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<MuebleListDto>>.Success(result.Value)
            : Result<List<MuebleListDto>>.Error("Error al obtener muebles");
    }

    private static MuebleDetailDto MapEntityToDetailDto(MuebleEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Ubicacion = entity.Ubicacion,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static MuebleDetailDto MapListDtoToDetailDto(MuebleListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Ubicacion = dto.Ubicacion,
        EstadoEliminado = false
    };
}
