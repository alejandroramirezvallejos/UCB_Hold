using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;

namespace IMT_Reservas.Server.Application.Features.Mueble.Validators;

public class MuebleValidator : Validator<MuebleEntity>
{
    public override Result<object> Validate(MuebleEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
