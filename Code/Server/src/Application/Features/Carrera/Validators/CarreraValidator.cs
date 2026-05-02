using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;

namespace IMT_Reservas.Server.Application.Features.Carrera.Validators;

public class CarreraValidator : Validator<CarreraEntity>
{
    public override Result<object> Validate(CarreraEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
