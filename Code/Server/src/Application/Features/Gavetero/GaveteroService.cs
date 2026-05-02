using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Application.Features.Gavetero.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService 
{
private readonly IRepository<GaveteroListDto> _repository;
	private readonly IMapper _mapper;

	public GaveteroService(IRepository<GaveteroListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<GaveteroDetailDto>> CreateAsync(GaveteroEntity entity)
	{
		var validationResult = GaveteroValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<GaveteroDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<GaveteroDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GaveteroDetailDto>.Success(_mapper.Map<GaveteroDetailDto>(result.Value));
	}

	public async Task<Result<GaveteroDetailDto>> UpdateAsync(GaveteroEntity entity)
	{
		var validationResult = GaveteroValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<GaveteroDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<GaveteroDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GaveteroDetailDto>.Success(_mapper.Map<GaveteroDetailDto>(result.Value));
	}

	public async Task<Result<List<GaveteroListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<GaveteroListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<GaveteroListDto>>(result.Value);
		return Result<List<GaveteroListDto>>.Success(dtos);
	}

	public async Task<Result<GaveteroDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<GaveteroDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GaveteroDetailDto>.Success(_mapper.Map<GaveteroDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(GaveteroEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["idMueble"] = entity.IdMueble
		};
	}

	protected int GetEntityId(GaveteroEntity entity) => entity.Id;
}

