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

    public async Task<Result<MantenimientoDetailDto>> Create(MantenimientoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<MantenimientoDetailDto>.Error("Error al crear mantenimiento")
            : Result<MantenimientoDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<MantenimientoDetailDto>> Update(MantenimientoEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<MantenimientoDetailDto>.Error("Error al actualizar mantenimiento")
            : Result<MantenimientoDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar mantenimiento");
    }

    public async Task<Result<MantenimientoDetailDto>> Get(int id)
    {
        var mantenimiento = await _repository.Get(id);
        
        return !mantenimiento.IsSuccess
            ? Result<MantenimientoDetailDto>.NotFound()
            : Result<MantenimientoDetailDto>.Success(MapListDtoToDetailDto(mantenimiento.Value));
    }

    public async Task<Result<List<MantenimientoListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<MantenimientoListDto>>.Success(result.Value)
            : Result<List<MantenimientoListDto>>.Error("Error al obtener mantenimientos");
    }
    
    private static MantenimientoDetailDto MapListDtoToDetailDto(MantenimientoListDto dto) => new()
    {
        Id = dto.Id,
        IdEquipo = 0,
        IdEmpresaMantenimiento = dto.IdEmpresa ?? 0,
        FechaInicio = dto.FechaMantenimiento ?? DateTime.Now,
        FechaFin = dto.FechaFinalDeMantenimiento ?? DateTime.Now,
        Descripcion = dto.Descripcion,
        Costo = dto.Costo,
        EstadoEliminado = false
    };
}
