using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Errors;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;

namespace IMT_Reservas.Server.Application.Features.Categoria.Validators;

public static class CategoriaValidator
{
	public static Result<object> ValidateCreate(CategoriaEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		return Result<object>.Success(true);
	}

	public static Result<object> ValidateUpdate(CategoriaEntity entity)
	{
		if (entity == null)
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity)));

		if (entity.Id <= 0)
			return Result<object>.Invalid(ErrorFactory.InvalidField<CategoriaEntity>(nameof(entity.Id)));

		var nombreTrimmed = entity.Nombre?.Trim();
		if (string.IsNullOrWhiteSpace(nombreTrimmed))
			return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Nombre)));

		if (nombreTrimmed.Length > 255)
			return Result<object>.Invalid(new ValidationError(nameof(entity.Nombre), "Max 255 characters"));

		return Result<object>.Success(true);
	}
}
