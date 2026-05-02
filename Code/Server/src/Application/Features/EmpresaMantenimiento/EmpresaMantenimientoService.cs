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

    public async Task<Result<EmpresaMantenimientoDetailDto>> Create(EmpresaMantenimientoEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<EmpresaMantenimientoDetailDto>.Error("Error al crear empresa de mantenimiento")
            : Result<EmpresaMantenimientoDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<EmpresaMantenimientoDetailDto>> Update(EmpresaMantenimientoEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<EmpresaMantenimientoDetailDto>.Error("Error al actualizar empresa de mantenimiento")
            : Result<EmpresaMantenimientoDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar empresa de mantenimiento");
    }

    public async Task<Result<EmpresaMantenimientoDetailDto>> Get(int id)
    {
        var empresa = await _repository.Get(id);
        return !empresa.IsSuccess
            ? Result<EmpresaMantenimientoDetailDto>.NotFound()
            : Result<EmpresaMantenimientoDetailDto>.Success(MapListDtoToDetailDto(empresa.Value));
    }

    public async Task<Result<List<EmpresaMantenimientoListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<EmpresaMantenimientoListDto>>.Success(result.Value)
            : Result<List<EmpresaMantenimientoListDto>>.Error("Error al obtener empresas de mantenimiento");
    }

    private static EmpresaMantenimientoDetailDto MapEntityToDetailDto(EmpresaMantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Contacto = $"{entity.NombreResponsable} {entity.ApellidoResponsable}",
        Email = null,
        Telefono = entity.Telefono,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static EmpresaMantenimientoDetailDto MapListDtoToDetailDto(EmpresaMantenimientoListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.NombreEmpresa,
        Contacto = $"{dto.NombreResponsable} {dto.ApellidoResponsable}",
        Email = dto.Email,
        Telefono = dto.Telefono,
        EstadoEliminado = false
    };
}
