using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Application.Features.Mueble.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService 
{
private readonly IRepository<MuebleListDto> _repository;
	private readonly IMapper _mapper;

	public MuebleService(IRepository<MuebleListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<MuebleDetailDto>> CreateAsync(MuebleEntity entity)
	{
		var validationResult = MuebleValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<MuebleDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<MuebleDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MuebleDetailDto>.Success(_mapper.Map<MuebleDetailDto>(result.Value));
	}

	public async Task<Result<MuebleDetailDto>> UpdateAsync(MuebleEntity entity)
	{
		var validationResult = MuebleValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<MuebleDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<MuebleDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MuebleDetailDto>.Success(_mapper.Map<MuebleDetailDto>(result.Value));
	}

	public async Task<Result<List<MuebleListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<MuebleListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<MuebleListDto>>(result.Value);
		return Result<List<MuebleListDto>>.Success(dtos);
	}

	public async Task<Result<MuebleDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<MuebleDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<MuebleDetailDto>.Success(_mapper.Map<MuebleDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(MuebleEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["ubicacion"] = entity.Ubicacion ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(MuebleEntity entity) => entity.Id;
}
