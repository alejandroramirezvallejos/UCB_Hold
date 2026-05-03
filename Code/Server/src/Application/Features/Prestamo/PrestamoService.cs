using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService
{
    private readonly PrestamoRepository _repository;

    public PrestamoService(PrestamoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PrestamoDto>> Create(PrestamoEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<PrestamoDto>> Update(PrestamoEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<PrestamoDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<PrestamoDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
