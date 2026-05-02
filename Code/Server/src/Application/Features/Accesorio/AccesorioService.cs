using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Accesorio.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService 
{
	private readonly IRepository<AccesorioListDto> _repository;
	private readonly IMapper _mapper;

	public AccesorioService(IRepository<AccesorioListDto> repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<AccesorioDetailDto>> CreateAsync(AccesorioEntity entity)
	{
		var validationResult = AccesorioValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<AccesorioDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var parameters = MapEntityToParameters(entity);
		var result = await _repository.CreateAsync(parameters);

		if (!result.IsSuccess)
			return Result<AccesorioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<AccesorioDetailDto>.Success(_mapper.Map<AccesorioDetailDto>(result.Value));
	}

	public async Task<Result<AccesorioDetailDto>> UpdateAsync(AccesorioEntity entity)
	{
		var validationResult = AccesorioValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<AccesorioDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var parameters = MapEntityToParameters(entity);
		var result = await _repository.UpdateAsync(parameters);

		if (!result.IsSuccess)
			return Result<AccesorioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<AccesorioDetailDto>.Success(_mapper.Map<AccesorioDetailDto>(result.Value));
	}

	public async Task<Result<List<AccesorioListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<AccesorioListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<List<AccesorioListDto>>.Success(result.Value);
	}

	public async Task<Result<AccesorioDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<AccesorioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<AccesorioDetailDto>.Success(_mapper.Map<AccesorioDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(AccesorioEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["descripcion"] = entity.Descripcion ?? (object)DBNull.Value,
			["modelo"] = entity.Modelo ?? (object)DBNull.Value,
			["urlDataSheet"] = entity.UrlDataSheet ?? (object)DBNull.Value,
			["precio"] = entity.Precio ?? (object)DBNull.Value,
			["idEquipo"] = entity.IdEquipo,
			["tipo"] = entity.Tipo ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(AccesorioEntity entity) => entity.Id;
}
