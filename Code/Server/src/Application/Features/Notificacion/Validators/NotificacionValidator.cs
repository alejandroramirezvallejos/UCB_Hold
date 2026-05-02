using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using NotificacionEntity = IMT_Reservas.Server.Core.Entities.Notificacion;

namespace IMT_Reservas.Server.Application.Features.Notificacion.Validators;

public static class NotificacionValidator
{
	public static Result<object> ValidateCreate(NotificacionEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.IdUsuario <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<NotificacionEntity>(nameof(entity.IdUsuario)));

		var tituloTrimmed = entity.Titulo?.Trim();
		if (string.IsNullOrWhiteSpace(tituloTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Titulo)));

		if (tituloTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Titulo), "Max 255 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(NotificacionEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<NotificacionEntity>(nameof(entity.Id)));

		if (entity.IdUsuario <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<NotificacionEntity>(nameof(entity.IdUsuario)));

		var tituloTrimmed = entity.Titulo?.Trim();
		if (string.IsNullOrWhiteSpace(tituloTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Titulo)));

		if (tituloTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Titulo), "Max 255 characters"));

		return Result<object>.Success(true);
	}
}
