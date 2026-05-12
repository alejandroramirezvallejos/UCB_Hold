using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Core.Entities;
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
        var prestamoIds = prestamos.Select(p => p.Id).ToHashSet();
        
        var detalles = await DbContext.DetallesPrestamos.AsNoTracking()
            .Where(d => prestamoIds.Contains(d.IdPrestamo))
            .ToListAsync();

        var carnets = prestamos.Select(p => p.Carnet).Where(_ => true).ToHashSet();
        
        var usuarioMap = await DbContext.Usuarios.AsNoTracking()
            .Where(u => carnets.Contains(u.Carnet))
            .ToDictionaryAsync(u => u.Carnet);

        var equipoIds = detalles.Select(d => d.IdEquipo).ToHashSet();
       
        var equipoMap = await DbContext.Equipos.AsNoTracking()
            .Include(e => e.GrupoEquipo)
            .Include(e => e.Gavetero).ThenInclude(g => g!.Mueble)
            .Where(e => equipoIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id);

        var detallesByPrestamo = detalles
            .GroupBy(d => d.IdPrestamo)
            .ToDictionary(g => g.Key, g => g.ToList());

        var dtos = new List<PrestamoDto>();
        
        foreach (var p in prestamos)
        {
            var usuario = usuarioMap.GetValueOrDefault(p.Carnet);

            if (!detallesByPrestamo.TryGetValue(p.Id, out var pDetalles))
            {
                dtos.Add(BuildDto(p, usuario, null, null, null, null));
                continue;
            }

            foreach (var d in pDetalles)
            {
                equipoMap.TryGetValue(d.IdEquipo, out var equipo);
                dtos.Add(BuildDto(p, usuario, equipo, equipo?.GrupoEquipo, equipo?.Gavetero, equipo?.Gavetero?.Mueble));
            }
        }

        return Result<List<PrestamoDto>>.Success(dtos);
    }

    public override async Task<Result<PrestamoDto>> Get(int id)
    {
        var p = await DbContext.Prestamos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        
        if (p == null) 
            return Result<PrestamoDto>.NotFound();

        var usuario = await DbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Carnet == p.Carnet);

        var detalle = await DbContext.DetallesPrestamos.AsNoTracking().FirstOrDefaultAsync(d => d.IdPrestamo == id);
        Equipo? equipo = null;
        GrupoEquipo? grupo = null;
        Gavetero? gavetero = null;
        Mueble? mueble = null;

        if (detalle != null)
        {
            equipo = await DbContext.Equipos.AsNoTracking()
                .Include(e => e.GrupoEquipo)
                .Include(e => e.Gavetero).ThenInclude(g => g!.Mueble)
                .FirstOrDefaultAsync(e => e.Id == detalle.IdEquipo);

            if (equipo != null)
            {
                grupo = equipo.GrupoEquipo;
                gavetero = equipo.Gavetero;
                mueble = gavetero?.Mueble;
            }
        }

        return Result<PrestamoDto>.Success(BuildDto(p, usuario, equipo, grupo, gavetero, mueble));
    }

    private static string MapEstado(EstadoPrestamo e) => e switch
    {
        EstadoPrestamo.Aprobado   => "aprobado",
        EstadoPrestamo.Activo     => "activo",
        EstadoPrestamo.Rechazado  => "rechazado",
        EstadoPrestamo.Finalizado => "finalizado",
        EstadoPrestamo.Cancelado  => "cancelado",
        _                         => "pendiente"
    };

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
        EstadoPrestamo = MapEstado(p.EstadoPrestamo),
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

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Prestamos
            .FirstOrDefaultAsync(p => p.Id == id && !p.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }
    
    public PrestamoDto ConvertToDto(PrestamoEntity entity) => MapToDto(entity);

    protected override PrestamoDto MapToDto(PrestamoEntity entity) => new()
    {
        Id = entity.Id,
        CarnetUsuario = entity.Carnet,
        EstadoPrestamo = MapEstado(entity.EstadoPrestamo),
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

