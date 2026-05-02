using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using ComentarioEntity = IMT_Reservas.Server.Core.Entities.Comentario;

namespace IMT_Reservas.Server.Application.Features.Comentario.Validators;

public static class ComentarioValidator
{
	public static Result<object> ValidateCreate(ComentarioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.IdGrupoEquipo <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<ComentarioEntity>(nameof(entity.IdGrupoEquipo)));

		if (entity.IdUsuario <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<ComentarioEntity>(nameof(entity.IdUsuario)));

		var contenidoTrimmed = entity.Contenido?.Trim();
		if (string.IsNullOrWhiteSpace(contenidoTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Contenido)));

		if (contenidoTrimmed.Length > 1000)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Contenido), "Max 1000 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(ComentarioEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<ComentarioEntity>(nameof(entity.Id)));

		var contenidoTrimmed = entity.Contenido?.Trim();
		if (string.IsNullOrWhiteSpace(contenidoTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Contenido)));

		if (contenidoTrimmed.Length > 1000)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Contenido), "Max 1000 characters"));

		return Result<object>.Success(true);
	}
}
