using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoValidator : AbstractValidator<GrupoEquipoDto>
{
    public GrupoEquipoValidator()
    {
        RuleFor(grupoEquipo => grupoEquipo.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(grupoEquipo => grupoEquipo.Modelo).NotEmpty().WithMessage("Modelo requerido");
        RuleFor(grupoEquipo => grupoEquipo.Marca).NotEmpty().WithMessage("Marca requerida");
        RuleFor(grupoEquipo => grupoEquipo.IdCategoria).NotNull().GreaterThan(0).WithMessage("IdCategoria requerido");
    }
}
