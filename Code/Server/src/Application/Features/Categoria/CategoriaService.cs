using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService
{
    private readonly CategoriaRepository _repository;

    public CategoriaService(CategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoriaDetail>> Create(CategoriaEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<CategoriaDetail>.Error("Error al crear categoría")
            : Result<CategoriaDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<CategoriaDetail>> Update(CategoriaEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<CategoriaDetail>.Error("Error al actualizar categoría")
            : Result<CategoriaDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar categoría");
    }

    public async Task<Result<CategoriaDetail>> Get(int id)
    {
        var categoria = await _repository.Get(id);
        
        return !categoria.IsSuccess
            ? Result<CategoriaDetail>.NotFound()
            : Result<CategoriaDetail>.Success(MapListToDetail(categoria.Value));
    }

    public async Task<Result<List<CategoriaList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<CategoriaList>>.Success(result.Value)
            : Result<List<CategoriaList>>.Error("Error al obtener categorías");
    }

    private static CategoriaDetail MapListToDetail(CategoriaList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        EstadoEliminado = false
    };
}
