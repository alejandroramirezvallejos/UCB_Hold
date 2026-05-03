using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService
{
    private readonly EmpresaMantenimientoRepository _repository;

    public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EmpresaMantenimientoDetail>> Create(EmpresaMantenimientoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<EmpresaMantenimientoDetail>.Error("Error al crear empresa de mantenimiento")
            : Result<EmpresaMantenimientoDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<EmpresaMantenimientoDetail>> Update(EmpresaMantenimientoEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<EmpresaMantenimientoDetail>.Error("Error al actualizar empresa de mantenimiento")
            : Result<EmpresaMantenimientoDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar empresa de mantenimiento");
    }

    public async Task<Result<EmpresaMantenimientoDetail>> Get(int id)
    {
        var empresa = await _repository.Get(id);
       
        return !empresa.IsSuccess
            ? Result<EmpresaMantenimientoDetail>.NotFound()
            : Result<EmpresaMantenimientoDetail>.Success(MapListToDetail(empresa.Value));
    }

    public async Task<Result<List<EmpresaMantenimientoList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<EmpresaMantenimientoList>>.Success(result.Value)
            : Result<List<EmpresaMantenimientoList>>.Error("Error al obtener empresas de mantenimiento");
    }

    private static EmpresaMantenimientoDetail MapListToDetail(EmpresaMantenimientoList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.NombreEmpresa,
        NombreResponsable = dto.NombreResponsable,
        ApellidoResponsable = dto.ApellidoResponsable,
        Telefono = dto.Telefono,
        Email = dto.Email,
        Nit = dto.Nit,
        Direccion = dto.Direccion,
        EstadoEliminado = false
    };
}
