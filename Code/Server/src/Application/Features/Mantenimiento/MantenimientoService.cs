using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService
{
    private readonly MantenimientoRepository _repository;

    public MantenimientoService(MantenimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MantenimientoDto>> Create(MantenimientoEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<MantenimientoDto>> Update(MantenimientoEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<MantenimientoDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<MantenimientoDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
