using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoEntity, MantenimientoDto>
{
    public MantenimientoRepository(ApplicationDbContext dbContext, MantenimientoMapper mapper)
        : base(dbContext, mapper) { }

    public override async Task<Result<List<MantenimientoDto>>> GetAll()
    {
        var rows = await (
            from mantenimiento in DbContext.Mantenimientos.AsNoTracking()
            join empresa in DbContext.EmpresasMantenimiento.AsNoTracking()
                on mantenimiento.IdEmpresa equals empresa.Id into empresaJoin
            from empresa in empresaJoin.DefaultIfEmpty()
            join detalle in DbContext.DetallesMantenimientos.AsNoTracking()
                on mantenimiento.Id equals detalle.IdMantenimiento into detalleJoin
            from detalle in detalleJoin.DefaultIfEmpty()
            join equipo in DbContext.Equipos.AsNoTracking()
                on detalle.IdEquipo equals equipo.Id into equipoJoin
            from equipo in equipoJoin.DefaultIfEmpty()
            join grupo in DbContext.GruposEquipos.AsNoTracking()
                on equipo.IdGrupoEquipo equals grupo.Id into grupoJoin
            from grupo in grupoJoin.DefaultIfEmpty()
            select new MantenimientoDto
            {
                Id = mantenimiento.Id,
                IdEmpresa = mantenimiento.IdEmpresa,
                NombreEmpresaMantenimiento = empresa != null ? empresa.Nombre : null,
                FechaMantenimiento = mantenimiento.FechaMantenimiento,
                FechaFinalMantenimiento = mantenimiento.FechaFinalMantenimiento,
                Costo = mantenimiento.Costo,
                Descripcion = mantenimiento.Descripcion,
                TipoMantenimiento = detalle != null ? detalle.TipoMantenimiento : null,
                CodigoImtEquipo = equipo != null ? equipo.CodigoImt.ToString() : null,
                NombreGrupoEquipo = grupo != null ? grupo.Nombre : null,
                DescripcionEquipo = detalle != null ? detalle.Descripcion : null
            }
        ).ToListAsync();

        return Result<List<MantenimientoDto>>.Success(rows);
    }

    public override async Task<Result<MantenimientoDto>> Get(int id)
    {
        var dto = await (
            from mantenimiento in DbContext.Mantenimientos.AsNoTracking()
            where mantenimiento.Id == id
            join empresa in DbContext.EmpresasMantenimiento.AsNoTracking()
                on mantenimiento.IdEmpresa equals empresa.Id into empresaJoin
            from empresa in empresaJoin.DefaultIfEmpty()
            select new MantenimientoDto
            {
                Id = mantenimiento.Id,
                IdEmpresa = mantenimiento.IdEmpresa,
                NombreEmpresaMantenimiento = empresa != null ? empresa.Nombre : null,
                FechaMantenimiento = mantenimiento.FechaMantenimiento,
                FechaFinalMantenimiento = mantenimiento.FechaFinalMantenimiento,
                Costo = mantenimiento.Costo,
                Descripcion = mantenimiento.Descripcion
            }
        ).FirstOrDefaultAsync();

        return dto == null ? Result<MantenimientoDto>.NotFound() : Result<MantenimientoDto>.Success(dto);
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Mantenimientos
            .FirstOrDefaultAsync(m => m.Id == id && !m.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }

    public async Task AddDetalles(int mantenimientoId, int[] codigosImt, string[]? tipos, string[]? descripciones)
    {
        for (var i = 0; i < codigosImt.Length; i++)
        {
            var equipo = await DbContext.Equipos
                .FirstOrDefaultAsync(e => e.CodigoImt == codigosImt[i] && !e.EstadoEliminado);

            if (equipo == null)
                continue;

            DbContext.DetallesMantenimientos.Add(new DetalleMantenimiento
            {
                IdMantenimiento = mantenimientoId,
                IdEquipo = equipo.Id,
                TipoMantenimiento = tipos?.ElementAtOrDefault(i),
                Descripcion = descripciones?.ElementAtOrDefault(i),
                EstadoEliminado = false
            });
        }

        await DbContext.SaveChangesAsync();
    }
}
