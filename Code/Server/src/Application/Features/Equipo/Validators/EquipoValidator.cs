using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;

namespace IMT_Reservas.Server.Application.Features.Equipo.Validators;

public class EquipoValidator : Validator<EquipoEntity>
{
    public override Result<object> Validate(EquipoEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdGrupoEquipo, nameof(entity.IdGrupoEquipo));
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.CodigoImt, nameof(entity.CodigoImt));
        if (!validation.IsSuccess) return validation;

        validation = RequiredString(entity.Modelo, nameof(entity.Modelo));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Modelo, nameof(entity.Modelo), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
