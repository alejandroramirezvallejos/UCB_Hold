using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService : Service<MantenimientoEntity, MantenimientoRepository, MantenimientoDto>
{
    private readonly ApplicationDbContext _dbContext;

    public MantenimientoService(MantenimientoRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _dbContext = dbContext;
    }

    public override async Task<Result<MantenimientoDto>> Create(MantenimientoEntity entity)
    {
        if (entity.Costo <= 0)
            return Result<MantenimientoDto>.Error("Costo debe ser mayor a 0");

        if (entity.FechaMantenimiento >= entity.FechaFinalMantenimiento)
            return Result<MantenimientoDto>.Error("Fecha mantenimiento menor a final");

        var empresaExists = await _dbContext.EmpresasMantenimiento
            .AnyAsync(e => e.Id == entity.IdEmpresa && !e.EstadoEliminado);

        if (!empresaExists)
            return Result<MantenimientoDto>.Error("Empresa mantenimiento no existe");

        return await base.Create(entity);
    }
}
