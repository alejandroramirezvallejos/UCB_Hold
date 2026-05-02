using Ardalis.Result;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Application.Features.Usuario.Validators;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

using AutoMapper;

namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService
{
	private readonly UsuarioRepository _repository;
	private readonly IMapper _mapper;

	public UsuarioService(UsuarioRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<UsuarioDetailDto>> CreateAsync(UsuarioEntity entity)
	{
		var validationResult = UsuarioValidator.ValidateCreate(entity);
		if (!validationResult.IsSuccess)
			return Result<UsuarioDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		entity.Email = entity.Email!.Trim();
		var result = await _repository.CreateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<UsuarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<UsuarioDetailDto>.Success(_mapper.Map<UsuarioDetailDto>(result.Value));
	}

	public async Task<Result<UsuarioDetailDto>> UpdateAsync(UsuarioEntity entity)
	{
		var validationResult = UsuarioValidator.ValidateUpdate(entity);
		if (!validationResult.IsSuccess)
			return Result<UsuarioDetailDto>.Error("Validation failed");

		entity.Nombre = entity.Nombre!.Trim();
		entity.Email = entity.Email!.Trim();
		var result = await _repository.UpdateAsync(MapEntityToParameters(entity));

		if (!result.IsSuccess)
			return Result<UsuarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<UsuarioDetailDto>.Success(_mapper.Map<UsuarioDetailDto>(result.Value));
	}

	public async Task<Result<List<UsuarioListDto>>> GetAllAsync(QueryFilter? filter = null)
	{
		var result = await _repository.GetAllAsync(filter);
		if (!result.IsSuccess)
			return Result<List<UsuarioListDto>>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		var dtos = _mapper.Map<List<UsuarioListDto>>(result.Value);
		return Result<List<UsuarioListDto>>.Success(dtos);
	}

	public async Task<Result<UsuarioDetailDto>> GetByIdAsync(int id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (!result.IsSuccess)
			return Result<UsuarioDetailDto>.Error(result.Errors.FirstOrDefault()?.ToString() ?? "Unknown error");

		return Result<UsuarioDetailDto>.Success(_mapper.Map<UsuarioDetailDto>(result.Value));
	}

	public async Task<Result<object>> DeleteAsync(int id)
	{
		var result = await _repository.DeleteAsync(id);
		return result;
	}

	public async Task<Result<UsuarioDetailDto>> LoginAsync(string email, string contrasena)
	{
		var result = await _repository.GetAllAsync(null);
		if (!result.IsSuccess)
			return Result<UsuarioDetailDto>.Error("Error al obtener usuarios");

		var usuario = result.Value?.FirstOrDefault(u =>
			u.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true);

		if (usuario == null)
			return Result<UsuarioDetailDto>.Error("Usuario no encontrado");

		var detailDto = new UsuarioDetailDto
		{
			Id = usuario.Id,
			Carnet = usuario.Carnet,
			Nombre = usuario.Nombre,
			ApellidoPaterno = usuario.ApellidoPaterno,
			ApellidoMaterno = usuario.ApellidoMaterno,
			Email = usuario.Email,
			Rol = usuario.Rol,
			IdCarrera = usuario.IdCarrera,
			Telefono = usuario.Telefono,
			TelefonoReferencia = usuario.TelefonoReferencia,
			NombreReferencia = usuario.NombreReferencia,
			EmailReferencia = usuario.EmailReferencia,
			EstadoEliminado = false
		};

		return Result<UsuarioDetailDto>.Success(detailDto);
	}

	protected Dictionary<string, object?> MapEntityToParameters(UsuarioEntity entity)
	{
		return new Dictionary<string, object?>
		{
			["id"] = entity.Id,
			["nombre"] = entity.Nombre ?? (object)DBNull.Value,
			["email"] = entity.Email ?? (object)DBNull.Value,
			["contrasena"] = entity.Contrasena ?? (object)DBNull.Value,
			["idCarrera"] = entity.IdCarrera ?? (object)DBNull.Value,
			["rol"] = entity.Rol ?? (object)DBNull.Value
		};
	}

	protected int GetEntityId(UsuarioEntity entity) => entity.Id;
}

