using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;

namespace IMT_Reservas.Server.Application.Features.Gavetero.Validators;

public class GaveteroValidator : Validator<GaveteroEntity>
{
    public override Result<object> Validate(GaveteroEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.IdMueble, nameof(entity.IdMueble));
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
