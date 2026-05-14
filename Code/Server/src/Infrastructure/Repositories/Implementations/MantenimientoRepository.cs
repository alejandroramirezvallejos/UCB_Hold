using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoEntity, MantenimientoDto>
{
    public MantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<MantenimientoDto>>> GetAll(QueryFilter? filter = null)
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
