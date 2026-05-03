using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService
{
    private readonly AccesorioRepository _repository;

    public AccesorioService(AccesorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AccesorioDto>> Create(AccesorioEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<AccesorioDto>> Update(AccesorioEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<AccesorioDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<AccesorioDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
