using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoValidator : AbstractValidator<EquipoDto>
{
    public EquipoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(e => e.IdGrupoEquipo)
            .Cascade(CascadeMode.Stop)
            .NotNull().GreaterThan(0).WithMessage("IdGrupoEquipo requerido")
            .MustAsync(async (id, _) => await dbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado))
            .WithMessage("Grupo equipo no existe");

        RuleFor(e => e.IdGavetero)
            .MustAsync(async (id, _) => await dbContext.Gaveteros.AnyAsync(g => g.Id == id && !g.EstadoEliminado))
            .When(e => e.IdGavetero.HasValue && e.IdGavetero.Value > 0)
            .WithMessage("Gavetero no existe");

        RuleFor(e => e.EstadoEquipo)
            .Must(estado => string.IsNullOrEmpty(estado)
                || new[] { "operativo", "parcialmente_operativo", "inoperativo" }.Contains(estado.ToLowerInvariant()))
            .WithMessage("Estado equipo inválido");
    }
}
