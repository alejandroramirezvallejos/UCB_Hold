using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using NotificacionEntity = IMT_Reservas.Server.Core.Entities.Notificacion;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Notificacion.Dtos;
using IMT_Reservas.Server.Application.Features.Notificacion.Validators;
using IMT_Reservas.Server.Infrastructure.Repositories;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Notificacion;

public class NotificacionService
{
	private readonly INotificacionRepository _repository;
	private readonly IMapper _mapper;

	public NotificacionService(INotificacionRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<NotificacionDetailDto>> CreateAsync(NotificacionEntity entity)
	{
		var validationResult = NotificacionValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<NotificacionDetailDto>.Error("Validation failed");

		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<NotificacionDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(result.Value));
	}

	public async Task<Result<NotificacionDetailDto>> UpdateAsync(NotificacionEntity entity)
	{
		var validationResult = NotificacionValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<NotificacionDetailDto>.Error("Validation failed");

		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<NotificacionDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(result.Value));
	}

	public async Task<Result<List<NotificacionListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<NotificacionListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<NotificacionListDto>>(result.Value);
		return Result<List<NotificacionListDto>>.Success(dtos);
	}

	public async Task<Result<NotificacionDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<NotificacionDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	protected Dictionary<string, object?> MapEntityToParameters(NotificacionEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["idUsuario"] = entity.IdUsuario,
			["titulo"] = entity.Titulo ?? (object)DBNull.Value,
			["contenido"] = entity.Contenido ?? (object)DBNull.Value,
			["esLeido"] = entity.EsLeido,
			["fechaCreacion"] = entity.FechaCreacion
		};
	}

	protected int GetEntityId(NotificacionEntity entity) => entity.Id;
}

