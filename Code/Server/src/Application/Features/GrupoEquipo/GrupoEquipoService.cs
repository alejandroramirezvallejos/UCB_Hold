using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService 
{
private readonly IRepository<GrupoEquipoListDto> _repository;
	private readonly IMapper _mapper;

	public GrupoEquipoService(IRepository<GrupoEquipoListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<GrupoEquipoDetailDto>> CreateAsync(GrupoEquipoEntity entity)
	{
		var validationResult = GrupoEquipoValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<GrupoEquipoDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<GrupoEquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GrupoEquipoDetailDto>.Success(_mapper.Map<GrupoEquipoDetailDto>(result.Value));
	}

	public async Task<Result<GrupoEquipoDetailDto>> UpdateAsync(GrupoEquipoEntity entity)
	{
		var validationResult = GrupoEquipoValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<GrupoEquipoDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<GrupoEquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GrupoEquipoDetailDto>.Success(_mapper.Map<GrupoEquipoDetailDto>(result.Value));
	}

	public async Task<Result<List<GrupoEquipoListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<GrupoEquipoListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<GrupoEquipoListDto>>(result.Value);
		return Result<List<GrupoEquipoListDto>>.Success(dtos);
	}

	public async Task<Result<GrupoEquipoDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<GrupoEquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<GrupoEquipoDetailDto>.Success(_mapper.Map<GrupoEquipoDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(GrupoEquipoEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["descripcion"] = entity.Descripcion ?? (object)DBNull.Value,
			["idCategoria"] = entity.IdCategoria
		};
	}

	protected int GetEntityId(GrupoEquipoEntity entity) => entity.Id;
}

