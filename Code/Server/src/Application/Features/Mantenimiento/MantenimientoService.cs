using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService
{
    private readonly MantenimientoRepository _repository;

    public MantenimientoService(MantenimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MantenimientoDetail>> Create(MantenimientoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<MantenimientoDetail>.Error("Error al crear mantenimiento")
            : Result<MantenimientoDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<MantenimientoDetail>> Update(MantenimientoEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<MantenimientoDetail>.Error("Error al actualizar mantenimiento")
            : Result<MantenimientoDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar mantenimiento");
    }

    public async Task<Result<MantenimientoDetail>> Get(int id)
    {
        var mantenimiento = await _repository.Get(id);
        
        return !mantenimiento.IsSuccess
            ? Result<MantenimientoDetail>.NotFound()
            : Result<MantenimientoDetail>.Success(MapListToDetail(mantenimiento.Value));
    }

    public async Task<Result<List<MantenimientoList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<MantenimientoList>>.Success(result.Value)
            : Result<List<MantenimientoList>>.Error("Error al obtener mantenimientos");
    }
    
    private static MantenimientoDetail MapListToDetail(MantenimientoList dto) => new()
    {
        Id = dto.Id,
        FechaMantenimiento = dto.FechaMantenimiento,
        FechaFinalDeMantenimiento = dto.FechaFinalDeMantenimiento,
        IdEmpresa = dto.IdEmpresa,
        Costo = dto.Costo,
        Descripcion = dto.Descripcion,
        EstadoEliminado = false
    };
}
