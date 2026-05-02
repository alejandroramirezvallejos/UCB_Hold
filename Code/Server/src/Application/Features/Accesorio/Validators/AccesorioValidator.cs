using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;

namespace IMT_Reservas.Server.Application.Features.Accesorio.Validators;

public static class AccesorioValidator
{
	public static Result<object> ValidateCreate(AccesorioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<AccesorioEntity>(nameof(entity.IdEquipo)));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(AccesorioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<AccesorioEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<AccesorioEntity>(nameof(entity.IdEquipo)));

		return Result<object>.Success(true);
	}
}
