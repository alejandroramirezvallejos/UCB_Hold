using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Application.Features.Componente.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService 
{
	private readonly IRepository<ComponenteListDto> _repository;
	private readonly IMapper _mapper;

	public ComponenteService(IRepository<ComponenteListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<ComponenteDetailDto>> CreateAsync(ComponenteEntity entity)
	{
		var validationResult = ComponenteValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<ComponenteDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<ComponenteDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComponenteDetailDto>.Success(_mapper.Map<ComponenteDetailDto>(result.Value));
	}

	public async Task<Result<ComponenteDetailDto>> UpdateAsync(ComponenteEntity entity)
	{
		var validationResult = ComponenteValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<ComponenteDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<ComponenteDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComponenteDetailDto>.Success(_mapper.Map<ComponenteDetailDto>(result.Value));
	}

	public async Task<Result<List<ComponenteListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<ComponenteListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<ComponenteListDto>>(result.Value);
		return Result<List<ComponenteListDto>>.Success(dtos);
	}

	public async Task<Result<ComponenteDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<ComponenteDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComponenteDetailDto>.Success(_mapper.Map<ComponenteDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(ComponenteEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["descripcion"] = entity.Descripcion ?? (object)DBNull.Value,
			["modelo"] = entity.Modelo ?? (object)DBNull.Value,
			["precio"] = entity.Precio ?? (object)DBNull.Value,
			["idEquipo"] = entity.IdEquipo
		};
	}

	protected int GetEntityId(ComponenteEntity entity) => entity.Id;
}
