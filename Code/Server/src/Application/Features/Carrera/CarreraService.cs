using Ardalis.Result;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera.Validators;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService
{
	private readonly CarreraRepository _repository;
	private readonly IMapper _mapper;

	public CarreraService(CarreraRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<CarreraDetailDto>> CreateAsync(CarreraEntity entity)
	{
		var validationResult = CarreraValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<CarreraDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<CarreraDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CarreraDetailDto>.Success(_mapper.Map<CarreraDetailDto>(result.Value));
	}

	public async Task<Result<CarreraDetailDto>> UpdateAsync(CarreraEntity entity)
	{
		var validationResult = CarreraValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<CarreraDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<CarreraDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CarreraDetailDto>.Success(_mapper.Map<CarreraDetailDto>(result.Value));
	}

	public async Task<Result<List<CarreraListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<CarreraListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<CarreraListDto>>(result.Value);
		return Result<List<CarreraListDto>>.Success(dtos);
	}

	public async Task<Result<CarreraDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<CarreraDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CarreraDetailDto>.Success(_mapper.Map<CarreraDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(CarreraEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(CarreraEntity entity) => entity.Id;
}

