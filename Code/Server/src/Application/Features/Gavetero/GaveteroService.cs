using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService
{
    private readonly GaveteroRepository _repository;

    public GaveteroService(GaveteroRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GaveteroDto>> Create(GaveteroEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<GaveteroDto>> Update(GaveteroEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<GaveteroDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<GaveteroDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
