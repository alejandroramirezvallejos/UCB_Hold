using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;

namespace IMT_Reservas.Server.Application.Features.Categoria.Validators;

public class CategoriaValidator : Validator<CategoriaEntity>
{
    public override Result<object> Validate(CategoriaEntity entity)
    {
        var validation = RequiredString(entity.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 255);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
