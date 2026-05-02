using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;

namespace IMT_Reservas.Server.Application.Features.Gavetero.Validators;

public static class GaveteroValidator
{
	public static Result<object> ValidateCreate(GaveteroEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdMueble <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GaveteroEntity>(nameof(entity.IdMueble)));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(GaveteroEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GaveteroEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdMueble <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GaveteroEntity>(nameof(entity.IdMueble)));

		return Result<object>.Success(true);
	}
}
