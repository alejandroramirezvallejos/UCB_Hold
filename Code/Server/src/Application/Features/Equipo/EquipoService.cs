using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Application.Features.Equipo.Validators;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService 
{
private readonly IRepository<EquipoListDto> _repository;
	private readonly IMapper _mapper;

	public EquipoService(IRepository<EquipoListDto> repository, IMapper mapper) 
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<EquipoDetailDto>> CreateAsync(EquipoEntity entity)
	{
		var validationResult = EquipoValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<EquipoDetailDto>.Error("Validation failed");

		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<EquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EquipoDetailDto>.Success(_mapper.Map<EquipoDetailDto>(result.Value));
	}

	public async Task<Result<EquipoDetailDto>> UpdateAsync(EquipoEntity entity)
	{
		var validationResult = EquipoValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<EquipoDetailDto>.Error("Validation failed");

		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<EquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EquipoDetailDto>.Success(_mapper.Map<EquipoDetailDto>(result.Value));
	}

	public async Task<Result<List<EquipoListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<EquipoListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<EquipoListDto>>(result.Value);
		return Result<List<EquipoListDto>>.Success(dtos);
	}

	public async Task<Result<EquipoDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<EquipoDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<EquipoDetailDto>.Success(_mapper.Map<EquipoDetailDto>(result.Value));
	}

	protected Dictionary<string, object?> MapEntityToParameters(EquipoEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["idGrupoEquipo"] = entity.IdGrupoEquipo,
			["codigoImt"] = entity.CodigoImt,
			["idGavetero"] = entity.IdGavetero ?? (object)DBNull.Value,
			["modelo"] = entity.Modelo ?? (object)DBNull.Value,
			["marca"] = entity.Marca ?? (object)DBNull.Value,
			["codigoUcb"] = entity.CodigoUcb ?? (object)DBNull.Value,
			["numeroSerial"] = entity.NumeroSerial ?? (object)DBNull.Value,
			["estadoEquipo"] = entity.EstadoEquipo ?? (object)DBNull.Value,
			["ubicacion"] = entity.Ubicacion ?? (object)DBNull.Value,
			["costoReferencia"] = entity.CostoReferencia ?? (object)DBNull.Value,
			["descripcion"] = entity.Descripcion ?? (object)DBNull.Value,
			["tiempoMaximoPrestamo"] = entity.TiempoMaximoPrestamo ?? (object)DBNull.Value,
			["procedencia"] = entity.Procedencia ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(EquipoEntity entity) => entity.Id;
}
