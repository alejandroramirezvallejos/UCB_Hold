using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService
{
    private readonly MuebleRepository _repository;

    public MuebleService(MuebleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MuebleDto>> Create(MuebleEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<MuebleDto>> Update(MuebleEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<MuebleDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<MuebleDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
