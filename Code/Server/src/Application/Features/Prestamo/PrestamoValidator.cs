using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoValidator : AbstractValidator<PrestamoDto>
{
    public PrestamoValidator()
    {
        RuleFor(prestamo => prestamo.CarnetUsuario).NotEmpty().WithMessage("Carnet de usuario requerido");
        RuleFor(prestamo => prestamo.FechaPrestamoEsperada).NotNull().WithMessage("Fecha préstamo esperada requerida");
        RuleFor(prestamo => prestamo.FechaDevolucionEsperada).NotNull().WithMessage("Fecha devolución esperada requerida");
        RuleFor(prestamo => prestamo)
            .Must(prestamo => !prestamo.FechaPrestamoEsperada.HasValue
                           || !prestamo.FechaDevolucionEsperada.HasValue
                           || prestamo.FechaPrestamoEsperada.Value <= prestamo.FechaDevolucionEsperada.Value)
            .WithMessage("Fecha préstamo no puede ser posterior a devolución");
    }
}
