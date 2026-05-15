using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoValidator : AbstractValidator<EquipoDto>
{
    public EquipoValidator()
    {
        RuleFor(equipo => equipo.IdGrupoEquipo).NotNull().GreaterThan(0).WithMessage("IdGrupoEquipo requerido");
        RuleFor(equipo => equipo.EstadoEquipo)
            .Must(estado => string.IsNullOrEmpty(estado) || new[] { "operativo", "parcialmente_operativo", "inoperativo" }.Contains(estado.ToLowerInvariant()))
            .WithMessage("Estado equipo inválido");
    }
}
