using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Application.Features.Prestamo.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService
{
	private readonly IRepository<PrestamoListDto> _repository;
	private readonly IMapper _mapper;

	public PrestamoService(IRepository<PrestamoListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<PrestamoDetailDto>> CreateAsync(PrestamoEntity entity)
	{
		var validationResult = PrestamoValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<PrestamoDetailDto>.Error("Validation failed");

		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<PrestamoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<PrestamoDetailDto>.Success(_mapper.Map<PrestamoDetailDto>(result.Value));
	}

	public async Task<Result<PrestamoDetailDto>> UpdateAsync(PrestamoEntity entity)
	{
		var validationResult = PrestamoValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<PrestamoDetailDto>.Error("Validation failed");

		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<PrestamoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<PrestamoDetailDto>.Success(_mapper.Map<PrestamoDetailDto>(result.Value));
	}

	public async Task<Result<List<PrestamoListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<PrestamoListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<PrestamoListDto>>(result.Value);
		return Result<List<PrestamoListDto>>.Success(dtos);
	}

	public async Task<Result<PrestamoDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<PrestamoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<PrestamoDetailDto>.Success(_mapper.Map<PrestamoDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(PrestamoEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["idUsuario"] = entity.IdUsuario,
			["fechaSolicitud"] = entity.FechaSolicitud,
			["fechaInicio"] = entity.FechaInicio,
			["fechaFin"] = entity.FechaFin,
			["estadoPrestamo"] = entity.EstadoPrestamo ?? (object)DBNull.Value,
			["observaciones"] = entity.Observaciones ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(PrestamoEntity entity) => entity.Id;
}
