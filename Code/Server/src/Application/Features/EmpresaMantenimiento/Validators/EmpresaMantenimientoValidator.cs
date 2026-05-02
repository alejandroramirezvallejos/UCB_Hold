using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;

namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Validators;

public class EmpresaMantenimientoValidator : Validator<EmpresaMantenimientoEntity>
{
    public override Result<object> Validate(EmpresaMantenimientoEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null!);
    }
}
