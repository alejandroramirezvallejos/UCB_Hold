using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoValidator : AbstractValidator<EquipoDto>
{
    private static readonly string[] ValidosEstados =
    [
        "operativo",
        "parcialmente_operativo",
        "inoperativo",
    ];

    public EquipoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(e => e.IdGrupoEquipo)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("IdGrupoEquipo requerido")
            .MustAsync(
                async (id, cancellationToken) =>
                    await dbContext.GruposEquipos.AnyAsync(
                        g => g.Id == id && !g.EstadoEliminado,
                        cancellationToken
                    )
            )
            .WithMessage("Grupo equipo no existe");

        RuleFor(e => e.IdGavetero)
            .MustAsync(
                async (id, cancellationToken) =>
                    await dbContext.Gaveteros.AnyAsync(
                        g => g.Id == id && !g.EstadoEliminado,
                        cancellationToken
                    )
            )
            .When(e => e.IdGavetero.HasValue && e.IdGavetero.Value > 0)
            .WithMessage("Gavetero no existe");

        RuleFor(e => e.EstadoEquipo)
            .Must(estado =>
                string.IsNullOrEmpty(estado) || ValidosEstados.Contains(estado.ToLowerInvariant())
            )
            .WithMessage("Estado equipo inválido");
    }
}
