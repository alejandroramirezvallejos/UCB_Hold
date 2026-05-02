using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using ComentarioEntity = IMT_Reservas.Server.Core.Entities.Comentario;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Comentario.Dtos;
using IMT_Reservas.Server.Application.Features.Comentario.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Comentario;

public class ComentarioService 
{
	private readonly IRepository<ComentarioListDto> _repository;
	private readonly IMapper _mapper;

	public ComentarioService(IRepository<ComentarioListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<ComentarioDetailDto>> CreateAsync(ComentarioEntity entity)
	{
		var validationResult = ComentarioValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<ComentarioDetailDto>.Error("Validation failed");

		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<ComentarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(result.Value));
	}

	public async Task<Result<ComentarioDetailDto>> UpdateAsync(ComentarioEntity entity)
	{
		var validationResult = ComentarioValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<ComentarioDetailDto>.Error("Validation failed");

		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<ComentarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(result.Value));
	}

	public async Task<Result<List<ComentarioListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<ComentarioListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<ComentarioListDto>>(result.Value);
		return Result<List<ComentarioListDto>>.Success(dtos);
	}

	public async Task<Result<ComentarioDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<ComentarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(ComentarioEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["contenido"] = entity.Contenido ?? (object)DBNull.Value,
			["idGrupoEquipo"] = entity.IdGrupoEquipo,
			["idUsuario"] = entity.IdUsuario,
			["cantidadLikes"] = entity.CantidadLikes,
			["fechaCreacion"] = entity.FechaCreacion
		};
	}

	protected int GetEntityId(ComentarioEntity entity) => entity.Id;
}

