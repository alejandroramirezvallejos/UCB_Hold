using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;

namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Validators;

public class GrupoEquipoValidator : Validator<GrupoEquipoEntity>
{
    public override Result<object> Validate(GrupoEquipoEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.IdCategoria, nameof(entity.IdCategoria));
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
