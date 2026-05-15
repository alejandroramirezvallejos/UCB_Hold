using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleValidator : AbstractValidator<MuebleDto>
{
    public MuebleValidator()
    {
        RuleFor(mueble => mueble.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(mueble => mueble.Costo).GreaterThanOrEqualTo(0).When(mueble => mueble.Costo.HasValue).WithMessage("Costo no puede ser negativo");
    }
}
