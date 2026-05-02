using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;

namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Validators;

public static class EmpresaMantenimientoValidator
{
	public static Result<object> ValidateCreate(EmpresaMantenimientoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(EmpresaMantenimientoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<EmpresaMantenimientoEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		return Result<object>.Success(true);
	}
}
