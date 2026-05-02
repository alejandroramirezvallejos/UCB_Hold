using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using ComentarioEntity = IMT_Reservas.Server.Core.Entities.Comentario;

namespace IMT_Reservas.Server.Application.Features.Comentario.Validators;

public class ComentarioValidator : Validator<ComentarioEntity>
{
    public override Result<object> Validate(ComentarioEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdGrupoEquipo, nameof(entity.IdGrupoEquipo));
        if (!validation.IsSuccess) return validation;

        validation = RequiredPositiveInt(entity.IdUsuario, nameof(entity.IdUsuario));
        if (!validation.IsSuccess) return validation;

        validation = RequiredString(entity.Contenido, nameof(entity.Contenido));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Contenido, nameof(entity.Contenido), 1000);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
