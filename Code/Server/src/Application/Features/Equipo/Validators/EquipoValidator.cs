using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;

namespace IMT_Reservas.Server.Application.Features.Equipo.Validators;

public static class EquipoValidator
{
	public static Result<object> ValidateCreate(EquipoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.IdGrupoEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EquipoEntity>(nameof(entity.IdGrupoEquipo)));

		if (entity.CodigoImt <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EquipoEntity>(nameof(entity.CodigoImt)));

		var modeloTrimmed = entity.Modelo?.Trim();
		if (string.IsNullOrWhiteSpace(modeloTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Modelo)));

		if (modeloTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Modelo), "Max 255 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(EquipoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EquipoEntity>(nameof(entity.Id)));

		if (entity.IdGrupoEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EquipoEntity>(nameof(entity.IdGrupoEquipo)));

		if (entity.CodigoImt <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EquipoEntity>(nameof(entity.CodigoImt)));

		var modeloTrimmed = entity.Modelo?.Trim();
		if (string.IsNullOrWhiteSpace(modeloTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Modelo)));

		if (modeloTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Modelo), "Max 255 characters"));

		return Result<object>.Success(true);
	}
}
