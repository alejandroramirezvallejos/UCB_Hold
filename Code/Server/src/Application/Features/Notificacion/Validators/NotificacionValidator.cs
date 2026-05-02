using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using NotificacionEntity = IMT_Reservas.Server.Core.Entities.Notificacion;

namespace IMT_Reservas.Server.Application.Features.Notificacion.Validators;

public class NotificacionValidator : Validator<NotificacionEntity>
{
    public override Result<object> Validate(NotificacionEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdUsuario, nameof(entity.IdUsuario));
        if (!validation.IsSuccess) return validation;

        validation = RequiredString(entity.Titulo, nameof(entity.Titulo));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Titulo, nameof(entity.Titulo), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
