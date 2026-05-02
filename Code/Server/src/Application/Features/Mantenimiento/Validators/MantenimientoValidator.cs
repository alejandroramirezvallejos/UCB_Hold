using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Validators;

public static class MantenimientoValidator
{
	public static Result<object> ValidateCreate(MantenimientoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.IdEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<MantenimientoEntity>(nameof(entity.IdEquipo)));

		if (entity.IdEmpresaMantenimiento <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<MantenimientoEntity>(nameof(entity.IdEmpresaMantenimiento)));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(MantenimientoEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<MantenimientoEntity>(nameof(entity.Id)));

		if (entity.IdEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<MantenimientoEntity>(nameof(entity.IdEquipo)));

		if (entity.IdEmpresaMantenimiento <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<MantenimientoEntity>(nameof(entity.IdEmpresaMantenimiento)));

		return Result<object>.Success(true);
	}
}
