using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroValidator : AbstractValidator<GaveteroDto>
{
    public GaveteroValidator(ApplicationDbContext dbContext)
    {
        RuleFor(gavetero => gavetero.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(gavetero => gavetero.NombreMueble).NotEmpty().WithMessage("NombreMueble requerido");

        RuleFor(gavetero => gavetero.NombreMueble)
            .MustAsync(async (nombre, _) => await dbContext.Muebles.AnyAsync(m => m.Nombre == nombre && !m.EstadoEliminado))
            .When(gavetero => !string.IsNullOrWhiteSpace(gavetero.NombreMueble))
            .WithMessage("Mueble no existe");
    }
}
