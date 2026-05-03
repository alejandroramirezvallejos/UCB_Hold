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

    public async Task<Result<GrupoEquipoDetail>> Create(GrupoEquipoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<GrupoEquipoDetail>.Error("Error al crear grupo de equipo")
            : Result<GrupoEquipoDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<GrupoEquipoDetail>> Update(GrupoEquipoEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<GrupoEquipoDetail>.Error("Error al actualizar grupo de equipo")
            : Result<GrupoEquipoDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar grupo de equipo");
    }

    public async Task<Result<GrupoEquipoDetail>> Get(int id)
    {
        var grupoEquipo = await _repository.Get(id);
        
        return !grupoEquipo.IsSuccess
            ? Result<GrupoEquipoDetail>.NotFound()
            : Result<GrupoEquipoDetail>.Success(MapListToDetail(grupoEquipo.Value));
    }

    public async Task<Result<List<GrupoEquipoList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
       
        return result.IsSuccess
            ? Result<List<GrupoEquipoList>>.Success(result.Value)
            : Result<List<GrupoEquipoList>>.Error("Error al obtener grupos de equipo");
    }
    
    private static GrupoEquipoDetail MapListToDetail(GrupoEquipoList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Descripcion = dto.Descripcion,
        IdCategoria = dto.IdCategoria ?? 0,
        EstadoEliminado = false
    };
}
