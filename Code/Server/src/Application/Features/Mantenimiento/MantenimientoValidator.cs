using FluentValidation;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoValidator : AbstractValidator<MantenimientoDto>
{
    public MantenimientoValidator(ApplicationDbContext dbContext)
    {
        RuleFor(mantenimiento => mantenimiento.IdEmpresa)
            .NotNull().GreaterThan(0).WithMessage("IdEmpresa requerido");

        RuleFor(mantenimiento => mantenimiento.IdEmpresa)
            .MustAsync(async (id, _) => await dbContext.EmpresasMantenimiento.AnyAsync(e => e.Id == id && !e.EstadoEliminado))
            .When(mantenimiento => (mantenimiento.IdEmpresa ?? 0) > 0)
            .WithMessage("Empresa mantenimiento no existe");

        RuleFor(mantenimiento => mantenimiento.FechaMantenimiento)
            .NotNull().WithMessage("Fecha mantenimiento requerida");

        RuleFor(mantenimiento => mantenimiento.FechaFinalMantenimiento)
            .NotNull().WithMessage("Fecha final requerida");

        RuleFor(mantenimiento => mantenimiento.Costo)
            .GreaterThanOrEqualTo(0).When(mantenimiento => mantenimiento.Costo.HasValue)
            .WithMessage("Costo no puede ser negativo");

        RuleFor(mantenimiento => mantenimiento)
            .Must(mantenimiento => !mantenimiento.FechaMantenimiento.HasValue
                                || !mantenimiento.FechaFinalMantenimiento.HasValue
                                || mantenimiento.FechaMantenimiento.Value <= mantenimiento.FechaFinalMantenimiento.Value)
            .WithMessage("Fecha inicio no puede ser posterior a fecha final");
    }
}
