using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService
{
    private readonly EmpresaMantenimientoRepository _repository;

    public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EmpresaMantenimientoDto>> Create(EmpresaMantenimientoEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<EmpresaMantenimientoDto>> Update(EmpresaMantenimientoEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<EmpresaMantenimientoDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<EmpresaMantenimientoDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
