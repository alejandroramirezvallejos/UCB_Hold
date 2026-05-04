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
        var entities = await DbContext.Mantenimientos
            .AsNoTracking()
            .ToListAsync();
        
        return Result<List<MantenimientoDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<MantenimientoDto>> Get(int id)
    {
        var entity = await DbContext.Mantenimientos
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return entity == null
            ? Result<MantenimientoDto>.NotFound()
            : Result<MantenimientoDto>.Success(MapToDto(entity));
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

