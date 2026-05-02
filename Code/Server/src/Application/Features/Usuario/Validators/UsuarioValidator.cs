using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;

namespace IMT_Reservas.Server.Application.Features.Usuario.Validators;

public static class UsuarioValidator
{
	public static Result<object> ValidateCreate(UsuarioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		var emailTrimmed = entity.Email?.Trim();
		if (string.IsNullOrWhiteSpace(emailTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Email)));

		if (emailTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Email), "Max 255 characters"));

		var contrasena = entity.Contrasena?.Trim();
		if (string.IsNullOrWhiteSpace(contrasena))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Contrasena)));

		if (contrasena.Length < 8)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Contrasena), "Min 8 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(UsuarioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<UsuarioEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		var emailTrimmed = entity.Email?.Trim();
		if (string.IsNullOrWhiteSpace(emailTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Email)));

		if (emailTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Email), "Max 255 characters"));

		return Result<object>.Success(true);
	}
}
