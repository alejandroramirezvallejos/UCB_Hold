using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;

namespace IMT_Reservas.Server.Application.Features.Accesorio.Validators;

public class AccesorioValidator : Validator<AccesorioEntity>
{
    public override Result<object> Validate(AccesorioEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.IdEquipo, nameof(entity.IdEquipo));
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
