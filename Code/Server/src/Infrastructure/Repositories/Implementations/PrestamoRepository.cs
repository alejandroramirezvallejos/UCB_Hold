using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoEntity, PrestamoDto>
{
    public PrestamoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<PrestamoDto>>> GetAll(QueryFilter? filter = null)
    {
        var prestamos = await DbContext.Prestamos.AsNoTracking().ToListAsync();
        var usuarios = await DbContext.Usuarios.AsNoTracking().ToListAsync();
        var detalles = await DbContext.DetallesPrestamos.AsNoTracking().ToListAsync();
        var equipos = await DbContext.Equipos.AsNoTracking().ToListAsync();
        var grupos = await DbContext.GruposEquipos.AsNoTracking().ToListAsync();
        var gaveteros = await DbContext.Gaveteros.AsNoTracking().ToListAsync();
        var muebles = await DbContext.Muebles.AsNoTracking().ToListAsync();

        var dtos = new List<PrestamoDto>();
        foreach (var p in prestamos)
        {
            var usuario = usuarios.FirstOrDefault(u => u.Carnet == p.Carnet);
            var pDetalles = detalles.Where(d => d.IdPrestamo == p.Id).ToList();

            if (!pDetalles.Any())
            {
                dtos.Add(BuildDto(p, usuario, null, null, null, null));
                continue;
            }

            foreach (var d in pDetalles)
            {
                var equipo = equipos.FirstOrDefault(e => e.Id == d.IdEquipo);
                var grupo = equipo != null ? grupos.FirstOrDefault(g => g.Id == equipo.IdGrupoEquipo) : null;
                var gavetero = equipo?.IdGavetero != null ? gaveteros.FirstOrDefault(g => g.Id == equipo.IdGavetero) : null;
                var mueble = gavetero != null ? muebles.FirstOrDefault(m => m.Id == gavetero.IdMueble) : null;
                dtos.Add(BuildDto(p, usuario, equipo, grupo, gavetero, mueble));
            }
        }

        return Result<List<PrestamoDto>>.Success(dtos);
    }

    public override async Task<Result<PrestamoDto>> Get(int id)
    {
        var p = await DbContext.Prestamos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Result<PrestamoDto>.NotFound();

        var usuario = await DbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Carnet == p.Carnet);

        return Result<PrestamoDto>.Success(BuildDto(p, usuario, null, null, null, null));
    }

    private static PrestamoDto BuildDto(
        PrestamoEntity p,
        Usuario? usuario,
        Equipo? equipo,
        GrupoEquipo? grupo,
        Gavetero? gavetero,
        Mueble? mueble) => new()
    {
        Id = p.Id,
        CarnetUsuario = p.Carnet,
        NombreUsuario = usuario?.Nombre,
        ApellidoPaternoUsuario = usuario?.ApellidoPaterno,
        TelefonoUsuario = usuario?.Telefono,
        EstadoPrestamo = p.EstadoPrestamo.ToDbString(),
        FechaSolicitud = p.FechaSolicitud,
        FechaPrestamoEsperada = p.FechaPrestamoEsperada,
        FechaPrestamo = p.FechaPrestamo,
        FechaDevolucionEsperada = p.FechaDevolucionEsperada,
        FechaDevolucion = p.FechaDevolucion,
        Observacion = p.Observacion,
        IdContrato = p.IdContrato,
        NombreGrupoEquipo = grupo?.Nombre,
        CodigoImt = equipo?.CodigoImt.ToString(),
        UbicacionEquipo = equipo?.Ubicacion,
        NombreGavetero = gavetero?.Nombre,
        NombreMueble = mueble?.Nombre,
        UbicacionMueble = mueble?.Ubicacion
    };

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Prestamos.AnyAsync(p => p.Id == id && !p.EstadoEliminado);

    public PrestamoDto ConvertToDto(PrestamoEntity entity) => MapToDto(entity);

    protected override PrestamoDto MapToDto(PrestamoEntity entity) => new()
    {
        Id = entity.Id,
        CarnetUsuario = entity.Carnet ?? "",
        EstadoPrestamo = entity.EstadoPrestamo.ToDbString(),
        FechaSolicitud = entity.FechaSolicitud,
        FechaDevolucionEsperada = entity.FechaDevolucionEsperada,
        NombreUsuario = null,
        ApellidoPaternoUsuario = null,
        TelefonoUsuario = null,
        FechaPrestamoEsperada = entity.FechaPrestamoEsperada,
        FechaPrestamo = entity.FechaPrestamo,
        FechaDevolucion = entity.FechaDevolucion,
        Observacion = entity.Observacion,
        IdContrato = entity.IdContrato,
        NombreGrupoEquipo = null,
        NombreGavetero = null
    };
}

