using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoValidator : AbstractValidator<GrupoEquipoDto>
{
    public GrupoEquipoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(grupoEquipo => grupoEquipo.Nombre).NotEmpty().WithMessage("Nombre requerido");
        RuleFor(grupoEquipo => grupoEquipo.Modelo).NotEmpty().WithMessage("Modelo requerido");
        RuleFor(grupoEquipo => grupoEquipo.Marca).NotEmpty().WithMessage("Marca requerida");

        RuleFor(grupoEquipo => grupoEquipo.IdCategoria)
            .NotNull().GreaterThan(0).WithMessage("IdCategoria requerido");

        RuleFor(grupoEquipo => grupoEquipo.IdCategoria)
            .MustAsync(async (id, _) => await dbContext.Categorias.AnyAsync(c => c.Id == id && !c.EstadoEliminado))
            .When(grupoEquipo => (grupoEquipo.IdCategoria ?? 0) > 0)
            .WithMessage("Categoría no existe");

        RuleFor(grupoEquipo => grupoEquipo)
            .MustAsync(async (dto, _) =>
            {
                var existing = await dbContext.GruposEquipos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Nombre == dto.Nombre && g.Modelo == dto.Modelo && g.Marca == dto.Marca && !g.EstadoEliminado);
                return existing == null || existing.Id == dto.Id;
            })
            .When(g => !string.IsNullOrEmpty(g.Nombre) && !string.IsNullOrEmpty(g.Modelo) && !string.IsNullOrEmpty(g.Marca))
            .WithMessage("Ya existe un grupo con ese nombre, modelo y marca");
    }
}
