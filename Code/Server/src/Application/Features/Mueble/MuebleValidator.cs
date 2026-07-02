using FluentValidation;

namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleValidator : AbstractValidator<MuebleDto>
{
    public MuebleValidator()
    {
        RuleFor(m => m.Nombre).NotEmpty().WithMessage("Nombre requerido");

        RuleFor(m => m.Costo)
            .GreaterThanOrEqualTo(0)
            .When(m => m.Costo.HasValue)
            .WithMessage("Costo no puede ser negativo");
    }
}
