using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;

namespace IMT_Reservas.Server.Application.Features.Prestamo.Validators;

public static class PrestamoValidator
{
	public static Result<object> ValidateCreate(PrestamoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.IdUsuario <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<PrestamoEntity>(nameof(entity.IdUsuario)));

		if (entity.FechaSolicitud == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaSolicitud)));

		if (entity.FechaInicio == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaInicio)));

		if (entity.FechaFin == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaFin)));

		if (entity.FechaFin <= entity.FechaInicio)
			return Result<object>.Invalid(new ValidationError(nameof(entity.FechaFin), "FechaFin must be after FechaInicio"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(PrestamoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<PrestamoEntity>(nameof(entity.Id)));

		if (entity.IdUsuario <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<PrestamoEntity>(nameof(entity.IdUsuario)));

		if (entity.FechaSolicitud == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaSolicitud)));

		if (entity.FechaInicio == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaInicio)));

		if (entity.FechaFin == default)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaFin)));

		if (entity.FechaFin <= entity.FechaInicio)
			return Result<object>.Invalid(new ValidationError(nameof(entity.FechaFin), "FechaFin must be after FechaInicio"));

		return Result<object>.Success(true);
	}
}
