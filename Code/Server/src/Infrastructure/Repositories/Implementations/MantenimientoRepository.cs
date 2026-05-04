using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoEntity, MantenimientoDto>
{
    public MantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Mantenimientos.AnyAsync(m => m.Id == id && !m.EstadoEliminado);

    public async Task<IEnumerable<MantenimientoEntity>> GetByEmpresa(int idEmpresa)
        => await DbContext.Mantenimientos
            .ToListAsync();

    public async Task<IEnumerable<MantenimientoEntity>> GetByDateRange(DateTime fechaInicio, DateTime fechaFin)
        => await DbContext.Mantenimientos
            .Where(m => m.FechaMantenimiento >= fechaInicio && m.FechaMantenimiento <= fechaFin && !m.EstadoEliminado)
            .ToListAsync();

    protected override MantenimientoDto MapToDto(MantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        IdEmpresa = entity.IdEmpresa,
        FechaMantenimiento = entity.FechaMantenimiento,
        FechaFinalMantenimiento = entity.FechaFinalMantenimiento,
        Costo = entity.Costo,
        Descripcion = entity.Descripcion
    };
}

