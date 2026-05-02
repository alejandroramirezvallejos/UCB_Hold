using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;

namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService
{
    private readonly GrupoEquipoRepository _repository;

    public GrupoEquipoService(GrupoEquipoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GrupoEquipoDetailDto>> Create(GrupoEquipoEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<GrupoEquipoDetailDto>.Error("Error al crear grupo de equipo")
            : Result<GrupoEquipoDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<GrupoEquipoDetailDto>> Update(GrupoEquipoEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<GrupoEquipoDetailDto>.Error("Error al actualizar grupo de equipo")
            : Result<GrupoEquipoDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null)
            : Result<object>.Error("Error al eliminar grupo de equipo");
    }

    public async Task<Result<GrupoEquipoDetailDto>> Get(int id)
    {
        var grupoEquipo = await _repository.Get(id);
        return !grupoEquipo.IsSuccess
            ? Result<GrupoEquipoDetailDto>.NotFound()
            : Result<GrupoEquipoDetailDto>.Success(MapListDtoToDetailDto(grupoEquipo.Value));
    }

    public async Task<Result<List<GrupoEquipoListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<GrupoEquipoListDto>>.Success(result.Value)
            : Result<List<GrupoEquipoListDto>>.Error("Error al obtener grupos de equipo");
    }

    private static GrupoEquipoDetailDto MapEntityToDetailDto(GrupoEquipoEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Descripcion = entity.Descripcion,
        IdCategoria = entity.IdCategoria,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static GrupoEquipoDetailDto MapListDtoToDetailDto(GrupoEquipoListDto dto) => new()
    {
        Id = dto.id,
        Nombre = dto.nombre,
        Descripcion = dto.descripcion,
        IdCategoria = dto.IdCategoria ?? 0,
        EstadoEliminado = false
    };
}
