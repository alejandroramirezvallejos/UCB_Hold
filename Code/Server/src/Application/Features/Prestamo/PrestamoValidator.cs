using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoValidator : AbstractValidator<PrestamoDto>
{
    public PrestamoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(prestamo => prestamo.CarnetUsuario)
            .NotEmpty().WithMessage("Carnet de usuario requerido");

        RuleFor(prestamo => prestamo.CarnetUsuario)
            .MustAsync(async (carnet, _) => await dbContext.Usuarios.AnyAsync(u => u.Carnet == carnet && !u.EstadoEliminado))
            .When(prestamo => !string.IsNullOrEmpty(prestamo.CarnetUsuario))
            .WithMessage("Usuario no existe o está inactivo");

        RuleFor(prestamo => prestamo.FechaPrestamoEsperada)
            .NotNull().WithMessage("Fecha préstamo esperada requerida");

        RuleFor(prestamo => prestamo.FechaDevolucionEsperada)
            .NotNull().WithMessage("Fecha devolución esperada requerida");

        RuleFor(prestamo => prestamo)
            .Must(prestamo => !prestamo.FechaPrestamoEsperada.HasValue
                           || !prestamo.FechaDevolucionEsperada.HasValue
                           || prestamo.FechaPrestamoEsperada.Value <= prestamo.FechaDevolucionEsperada.Value)
            .WithMessage("Fecha préstamo no puede ser posterior a devolución");
    }
}
