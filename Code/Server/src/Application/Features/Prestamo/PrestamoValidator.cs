using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoValidator : AbstractValidator<PrestamoDto>
{
    public PrestamoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(p => p.CarnetUsuario)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Carnet de usuario requerido")
            .MustAsync(
                async (carnet, cancellationToken) =>
                    await dbContext.Usuarios.AnyAsync(
                        u => u.Carnet == carnet && !u.EstadoEliminado,
                        cancellationToken
                    )
            )
            .WithMessage("Usuario no existe o está inactivo");

        RuleFor(p => p.FechaPrestamoEsperada)
            .NotNull()
            .WithMessage("Fecha préstamo esperada requerida");

        RuleFor(p => p.FechaDevolucionEsperada)
            .NotNull()
            .WithMessage("Fecha devolución esperada requerida");

        RuleFor(p => p)
            .Must(p =>
                !p.FechaPrestamoEsperada.HasValue
                || !p.FechaDevolucionEsperada.HasValue
                || p.FechaPrestamoEsperada.Value <= p.FechaDevolucionEsperada.Value
            )
            .WithMessage("Fecha préstamo no puede ser posterior a devolución");
    }
}
