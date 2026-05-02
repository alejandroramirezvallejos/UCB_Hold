using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;

namespace IMT_Reservas.Server.Application.Features.Usuario.Validators;

public class UsuarioValidator : Validator<UsuarioEntity>
{
    public override Result<object> Validate(UsuarioEntity entity)
    {
        var validation = RequiredString(entity?.Nombre, nameof(entity.Nombre));
        if (!validation.IsSuccess) return validation;

        validation = MaxLength(entity.Nombre, nameof(entity.Nombre), 64);
        if (!validation.IsSuccess) return validation;

        validation = RequiredString(entity?.Email, nameof(entity.Email));
        if (!validation.IsSuccess) return validation;

        validation = ValidEmail(entity.Email);
        if (!validation.IsSuccess) return validation;

        validation = RequiredString(entity?.Contrasena, nameof(entity.Contrasena));
        if (!validation.IsSuccess) return validation;

        validation = MinLength(entity.Contrasena, nameof(entity.Contrasena), 8);
        if (!validation.IsSuccess) return validation;

        return Result<object>.Success(null);
    }
}
