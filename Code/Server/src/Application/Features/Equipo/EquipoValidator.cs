using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoValidator : AbstractValidator<EquipoDto>
{
    public EquipoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(equipo => equipo.IdGrupoEquipo)
            .NotNull().GreaterThan(0).WithMessage("IdGrupoEquipo requerido");

        RuleFor(equipo => equipo.IdGrupoEquipo)
            .MustAsync(async (id, _) => await dbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado))
            .When(equipo => (equipo.IdGrupoEquipo ?? 0) > 0)
            .WithMessage("Grupo equipo no existe");

        RuleFor(equipo => equipo.IdGavetero)
            .MustAsync(async (id, _) => await dbContext.Gaveteros.AnyAsync(g => g.Id == id && !g.EstadoEliminado))
            .When(equipo => equipo.IdGavetero.HasValue && equipo.IdGavetero.Value > 0)
            .WithMessage("Gavetero no existe");

        RuleFor(equipo => equipo.EstadoEquipo)
            .Must(estado => string.IsNullOrEmpty(estado) || new[] { "operativo", "parcialmente_operativo", "inoperativo" }.Contains(estado.ToLowerInvariant()))
            .WithMessage("Estado equipo inválido");
    }
}
