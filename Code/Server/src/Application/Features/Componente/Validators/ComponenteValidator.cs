using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;

namespace IMT_Reservas.Server.Application.Features.Componente.Validators;

public class ComponenteValidator : Validator<ComponenteEntity>
{
    public override Result<object> Validate(ComponenteEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
