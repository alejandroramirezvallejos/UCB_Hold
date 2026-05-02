using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoEntity, MantenimientoListDto>
{
    public MantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Mantenimientos.AnyAsync(m => m.Id == id && !m.EstadoEliminado);

    public async Task<IEnumerable<MantenimientoEntity>> GetByEmpresa(int idEmpresa)
        => await DbContext.Mantenimientos
            .Where(m => m.IdEmpresa == idEmpresa && !m.EstadoEliminado)
            .ToListAsync();

    public async Task<IEnumerable<MantenimientoEntity>> GetByDateRange(DateTime fechaInicio, DateTime fechaFin)
        => await DbContext.Mantenimientos
            .Where(m => m.FechaMantenimiento >= fechaInicio && m.FechaMantenimiento <= fechaFin && !m.EstadoEliminado)
            .ToListAsync();

    protected override MantenimientoListDto MapToDto(MantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        FechaMantenimiento = entity.FechaMantenimiento,
        FechaFinalDeMantenimiento = entity.FechaFinalMantenimiento,
        IdEmpresa = entity.IdEmpresa,
        Costo = (decimal?)entity.Costo,
        Descripcion = entity.Descripcion,
        NombreEmpresaMantenimiento = null,
        TipoMantenimiento = null,
        NombreGrupoEquipo = null,
        CodigoImtEquipo = null,
        DescripcionEquipo = null
    };
}
