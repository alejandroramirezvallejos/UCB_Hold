using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioValidator : AbstractValidator<UsuarioDto>
{
    public UsuarioValidator()
    {
        RuleFor(usuario => usuario.Carnet).NotEmpty().WithMessage("Carnet requerido");
        RuleFor(usuario => usuario.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(usuario => usuario.ApellidoPaterno).NotEmpty().WithMessage("Apellido paterno requerido");
        RuleFor(usuario => usuario.Email).NotEmpty().EmailAddress().WithMessage("Email inválido");
        RuleFor(usuario => usuario.Contrasena).NotEmpty().MinimumLength(6).WithMessage("Contraseña mínimo 6 caracteres");
    }
}
