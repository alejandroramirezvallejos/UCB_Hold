using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Application.Features.Categoria.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService 
{
	private readonly IRepository<CategoriaListDto> _repository;
	private readonly IMapper _mapper;

	public CategoriaService(IRepository<CategoriaListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<CategoriaDetailDto>> CreateAsync(CategoriaEntity entity)
	{
		var validationResult = CategoriaValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<CategoriaDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<CategoriaDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CategoriaDetailDto>.Success(_mapper.Map<CategoriaDetailDto>(result.Value));
	}

	public async Task<Result<CategoriaDetailDto>> UpdateAsync(CategoriaEntity entity)
	{
		var validationResult = CategoriaValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<CategoriaDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<CategoriaDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CategoriaDetailDto>.Success(_mapper.Map<CategoriaDetailDto>(result.Value));
	}

	public async Task<Result<List<CategoriaListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<CategoriaListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<CategoriaListDto>>(result.Value);
		return Result<List<CategoriaListDto>>.Success(dtos);
	}

	public async Task<Result<CategoriaDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<CategoriaDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<CategoriaDetailDto>.Success(_mapper.Map<CategoriaDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(CategoriaEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(CategoriaEntity entity) => entity.Id;
}

