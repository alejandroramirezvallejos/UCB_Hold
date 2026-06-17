using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioValidator : AbstractValidator<UsuarioDto>
{
    public UsuarioValidator(ApplicationDbContext dbContext)
    {
        RuleFor(usuario => usuario.Carnet).NotEmpty().WithMessage("Carnet requerido");
        RuleFor(usuario => usuario.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(usuario => usuario.ApellidoPaterno).NotEmpty().WithMessage("Apellido paterno requerido");
        RuleFor(usuario => usuario.Email).NotEmpty().EmailAddress().WithMessage("Email inválido");
        RuleFor(usuario => usuario.Contrasena)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(8).WithMessage("Contraseña mínimo 8 caracteres")
            .Matches("[A-Z]").WithMessage("Contraseña debe tener al menos una mayúscula")
            .Matches("[0-9]").WithMessage("Contraseña debe tener al menos un número")
            .Matches("[^a-zA-Z0-9]").WithMessage("Contraseña debe tener al menos un carácter especial")
            .When(usuario => !string.IsNullOrWhiteSpace(usuario.Contrasena));

        RuleFor(usuario => usuario.IdCarrera)
            .MustAsync(async (id, cancellationToken) => await dbContext.Carreras.AnyAsync(c => c.Id == id && !c.EstadoEliminado, cancellationToken))
            .When(usuario => (usuario.IdCarrera ?? 0) > 0)
            .WithMessage("Carrera no existe");
    }
}
