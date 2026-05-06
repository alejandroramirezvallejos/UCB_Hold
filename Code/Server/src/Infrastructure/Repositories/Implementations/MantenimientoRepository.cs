using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoEntity, MantenimientoDto>
{
    public MantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<MantenimientoDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Mantenimientos
            .AsNoTracking()
            .Select(e => new MantenimientoDto
            {
                Id = e.Id,
                IdEmpresa = e.IdEmpresa,
                FechaMantenimiento = e.FechaMantenimiento,
                FechaFinalMantenimiento = e.FechaFinalMantenimiento,
                Costo = e.Costo,
                Descripcion = e.Descripcion
            })
            .ToListAsync();

        return Result<List<MantenimientoDto>>.Success(dtos);
    }

    public override async Task<Result<MantenimientoDto>> Get(int id)
    {
        var dto = await DbContext.Mantenimientos
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(e => new MantenimientoDto
            {
                Id = e.Id,
                IdEmpresa = e.IdEmpresa,
                FechaMantenimiento = e.FechaMantenimiento,
                FechaFinalMantenimiento = e.FechaFinalMantenimiento,
                Costo = e.Costo,
                Descripcion = e.Descripcion
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<MantenimientoDto>.NotFound()
            : Result<MantenimientoDto>.Success(dto);
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Mantenimientos.AnyAsync(m => m.Id == id && !m.EstadoEliminado);

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

