using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Core.Abstraction;
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
        var mantenimientos = await DbContext.Mantenimientos.AsNoTracking().ToListAsync();
        var empresas = await DbContext.EmpresasMantenimiento.AsNoTracking().ToListAsync();
        var detalles = await DbContext.DetallesMantenimientos.AsNoTracking().ToListAsync();
        var equipos = await DbContext.Equipos.AsNoTracking().ToListAsync();
        var grupos = await DbContext.GruposEquipos.AsNoTracking().ToListAsync();
        var dtos = new List<MantenimientoDto>();
        
        foreach (var m in mantenimientos)
        {
            var empresa = empresas.FirstOrDefault(e => e.Id == m.IdEmpresa);
            var mDetalles = detalles.Where(d => d.IdMantenimiento == m.Id).ToList();

            if (!mDetalles.Any())
            {
                dtos.Add(new MantenimientoDto
                {
                    Id = m.Id,
                    IdEmpresa = m.IdEmpresa,
                    NombreEmpresaMantenimiento = empresa?.Nombre,
                    FechaMantenimiento = m.FechaMantenimiento,
                    FechaFinalMantenimiento = m.FechaFinalMantenimiento,
                    Costo = m.Costo,
                    Descripcion = m.Descripcion
                });
                continue;
            }

            foreach (var d in mDetalles)
            {
                var equipo = equipos.FirstOrDefault(e => e.Id == d.IdEquipo);
                var grupo = equipo != null ? grupos.FirstOrDefault(g => g.Id == equipo.IdGrupoEquipo) : null;
                
                dtos.Add(new MantenimientoDto
                {
                    Id = m.Id,
                    IdEmpresa = m.IdEmpresa,
                    NombreEmpresaMantenimiento = empresa?.Nombre,
                    FechaMantenimiento = m.FechaMantenimiento,
                    FechaFinalMantenimiento = m.FechaFinalMantenimiento,
                    Costo = m.Costo,
                    Descripcion = m.Descripcion,
                    TipoMantenimiento = d.TipoMantenimiento,
                    CodigoImtEquipo = equipo?.CodigoImt.ToString(),
                    NombreGrupoEquipo = grupo?.Nombre,
                    DescripcionEquipo = d.Descripcion
                });
            }
        }

        return Result<List<MantenimientoDto>>.Success(dtos);
    }

    public override async Task<Result<MantenimientoDto>> Get(int id)
    {
        var m = await DbContext.Mantenimientos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        
        if (m == null) 
            return Result<MantenimientoDto>.NotFound();

        var empresa = await DbContext.EmpresasMantenimiento.AsNoTracking().FirstOrDefaultAsync(e => e.Id == m.IdEmpresa);

        return Result<MantenimientoDto>.Success(new MantenimientoDto
        {
            Id = m.Id,
            IdEmpresa = m.IdEmpresa,
            NombreEmpresaMantenimiento = empresa?.Nombre,
            FechaMantenimiento = m.FechaMantenimiento,
            FechaFinalMantenimiento = m.FechaFinalMantenimiento,
            Costo = m.Costo,
            Descripcion = m.Descripcion
        });
    }
    
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

