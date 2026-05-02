using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Validators;

public class MantenimientoValidator : Validator<MantenimientoEntity>
{
    public override Result<object> Validate(MantenimientoEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdEmpresa, nameof(entity.IdEmpresa));
        if (!validation.IsSuccess) return validation;

        if (entity.FechaMantenimiento == default)
            return Result<object>.Invalid(Core.Errors.ErrorFactory.RequiredField(nameof(entity.FechaMantenimiento)));

        if (entity.FechaFinalMantenimiento == default)
            return Result<object>.Invalid(Core.Errors.ErrorFactory.RequiredField(nameof(entity.FechaFinalMantenimiento)));

        return Result<object>.Success(null);
    }
}
