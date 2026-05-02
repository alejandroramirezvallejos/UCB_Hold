using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService 
{
private readonly IRepository<MantenimientoListDto> _repository;
	private readonly IMapper _mapper;

	public MantenimientoService(IRepository<MantenimientoListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<MantenimientoDetailDto>> CreateAsync(MantenimientoEntity entity)
	{
		var validationResult = MantenimientoValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<MantenimientoDetailDto>.Error("Validation failed");

		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<MantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MantenimientoDetailDto>.Success(_mapper.Map<MantenimientoDetailDto>(result.Value));
	}

	public async Task<Result<MantenimientoDetailDto>> UpdateAsync(MantenimientoEntity entity)
	{
		var validationResult = MantenimientoValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<MantenimientoDetailDto>.Error("Validation failed");

		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<MantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MantenimientoDetailDto>.Success(_mapper.Map<MantenimientoDetailDto>(result.Value));
	}

	public async Task<Result<List<MantenimientoListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<MantenimientoListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<MantenimientoListDto>>(result.Value);
		return Result<List<MantenimientoListDto>>.Success(dtos);
	}

	public async Task<Result<MantenimientoDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<MantenimientoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MantenimientoDetailDto>.Success(_mapper.Map<MantenimientoDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(MantenimientoEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["idEquipo"] = entity.IdEquipo,
			["idEmpresaMantenimiento"] = entity.IdEmpresaMantenimiento,
			["fechaInicio"] = entity.FechaInicio ?? (object)DBNull.Value,
			["fechaFin"] = entity.FechaFin ?? (object)DBNull.Value,
			["descripcion"] = entity.Descripcion ?? (object)DBNull.Value,
			["costo"] = entity.Costo ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(MantenimientoEntity entity) => entity.Id;
}
