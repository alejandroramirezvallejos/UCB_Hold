using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;

namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Validators;

public static class GrupoEquipoValidator
{
	public static Result<object> ValidateCreate(GrupoEquipoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdCategoria <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GrupoEquipoEntity>(nameof(entity.IdCategoria)));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(GrupoEquipoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GrupoEquipoEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		if (entity.IdCategoria <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<GrupoEquipoEntity>(nameof(entity.IdCategoria)));

		return Result<object>.Success(true);
	}
}
