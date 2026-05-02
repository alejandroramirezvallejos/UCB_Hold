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

    public async Task<Result<CategoriaDetailDto>> Create(CategoriaEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<CategoriaDetailDto>.Error("Error al crear categoría")
            : Result<CategoriaDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<CategoriaDetailDto>> Update(CategoriaEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<CategoriaDetailDto>.Error("Error al actualizar categoría")
            : Result<CategoriaDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar categoría");
    }

    public async Task<Result<CategoriaDetailDto>> Get(int id)
    {
        var categoria = await _repository.Get(id);
        
        return !categoria.IsSuccess
            ? Result<CategoriaDetailDto>.NotFound()
            : Result<CategoriaDetailDto>.Success(MapListDtoToDetailDto(categoria.Value));
    }

    public async Task<Result<List<CategoriaListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<CategoriaListDto>>.Success(result.Value)
            : Result<List<CategoriaListDto>>.Error("Error al obtener categorías");
    }

    private static CategoriaDetailDto MapListDtoToDetailDto(CategoriaListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        EstadoEliminado = false
    };
}
