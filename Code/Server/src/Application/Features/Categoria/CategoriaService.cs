using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService
{
    private readonly CategoriaRepository _repository;

    public CategoriaService(CategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoriaDto>> Create(CategoriaEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<CategoriaDto>> Update(CategoriaEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<CategoriaDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<CategoriaDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
