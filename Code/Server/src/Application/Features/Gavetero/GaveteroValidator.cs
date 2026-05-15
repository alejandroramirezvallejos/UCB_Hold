using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroValidator : AbstractValidator<GaveteroDto>
{
    public GaveteroValidator()
    {
        RuleFor(gavetero => gavetero.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(gavetero => gavetero.NombreMueble).NotEmpty().WithMessage("NombreMueble requerido");
    }
}
