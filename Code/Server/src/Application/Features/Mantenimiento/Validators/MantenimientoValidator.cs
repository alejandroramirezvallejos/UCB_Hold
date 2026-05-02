using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Validators;

public class MantenimientoValidator : Validator<MantenimientoEntity>
{
    public override Result<object> Validate(MantenimientoEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdEquipo, nameof(entity.IdEquipo));
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.IdEmpresaMantenimiento, nameof(entity.IdEmpresaMantenimiento));
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
