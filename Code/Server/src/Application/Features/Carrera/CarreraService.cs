using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService
{
    private readonly CarreraRepository _repository;

    public CarreraService(CarreraRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CarreraDto>> Create(CarreraEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<CarreraDto>> Update(CarreraEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<CarreraDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<CarreraDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
