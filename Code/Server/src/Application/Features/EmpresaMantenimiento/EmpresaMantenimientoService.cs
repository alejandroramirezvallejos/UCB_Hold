using Ardalis.Result;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Validators;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService
{
private readonly EmpresaMantenimientoRepository _repository;
	private readonly IMapper _mapper;

	public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<EmpresaMantenimientoDetailDto>> CreateAsync(EmpresaMantenimientoEntity entity)
	{
		var validationResult = EmpresaMantenimientoValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<EmpresaMantenimientoDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<EmpresaMantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EmpresaMantenimientoDetailDto>.Success(_mapper.Map<EmpresaMantenimientoDetailDto>(result.Value));
	}

	public async Task<Result<EmpresaMantenimientoDetailDto>> UpdateAsync(EmpresaMantenimientoEntity entity)
	{
		var validationResult = EmpresaMantenimientoValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<EmpresaMantenimientoDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<EmpresaMantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EmpresaMantenimientoDetailDto>.Success(_mapper.Map<EmpresaMantenimientoDetailDto>(result.Value));
	}

	public async Task<Result<List<EmpresaMantenimientoListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<EmpresaMantenimientoListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<EmpresaMantenimientoListDto>>(result.Value);
		return Result<List<EmpresaMantenimientoListDto>>.Success(dtos);
	}

	public async Task<Result<EmpresaMantenimientoDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<EmpresaMantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EmpresaMantenimientoDetailDto>.Success(_mapper.Map<EmpresaMantenimientoDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(EmpresaMantenimientoEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["contacto"] = entity.Contacto ?? (object)DBNull.Value,
			["email"] = entity.Email ?? (object)DBNull.Value,
			["telefono"] = entity.Telefono ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(EmpresaMantenimientoEntity entity) => entity.Id;
}

