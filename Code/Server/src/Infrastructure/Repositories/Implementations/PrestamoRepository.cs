using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
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
        var prestamoIds = prestamos.Select(prestamo => prestamo.Id).ToHashSet();

        var detalles = await DbContext.DetallesPrestamos.AsNoTracking()
            .Where(detalle => prestamoIds.Contains(detalle.IdPrestamo))
            .ToListAsync();

        var carnets = prestamos.Select(prestamo => prestamo.Carnet).Where(_ => true).ToHashSet();

        var usuarioMap = await DbContext.Usuarios.AsNoTracking()
            .Where(usuario => carnets.Contains(usuario.Carnet))
            .ToDictionaryAsync(usuario => usuario.Carnet);

        var equipoIds = detalles.Select(detalle => detalle.IdEquipo).ToHashSet();

        var equipoMap = await DbContext.Equipos.AsNoTracking()
            .Include(equipo => equipo.GrupoEquipo)
            .Include(equipo => equipo.Gavetero).ThenInclude(gavetero => gavetero!.Mueble)
            .Where(equipo => equipoIds.Contains(equipo.Id))
            .ToDictionaryAsync(equipo => equipo.Id);

        var detallesByPrestamo = detalles
            .GroupBy(detalle => detalle.IdPrestamo)
            .ToDictionary(agrupacion => agrupacion.Key, agrupacion => agrupacion.ToList());

        var dtos = new List<PrestamoDto>();

        foreach (var prestamo in prestamos)
        {
            var usuario = usuarioMap.GetValueOrDefault(prestamo.Carnet);

            if (!detallesByPrestamo.TryGetValue(prestamo.Id, out var pDetalles))
            {
                dtos.Add(BuildDto(prestamo, usuario, null, null, null, null));
                continue;
            }

            foreach (var detalle in pDetalles)
            {
                equipoMap.TryGetValue(detalle.IdEquipo, out var equipo);
                dtos.Add(BuildDto(prestamo, usuario, equipo, equipo?.GrupoEquipo, equipo?.Gavetero, equipo?.Gavetero?.Mueble));
            }
        }

        return Result<List<PrestamoDto>>.Success(dtos);
    }

    public async Task<Result<List<PrestamoDto>>> GetHistorialWithDetalles(string carnetUsuario, EstadoPrestamo? estado)
    {
        var prestamosQuery = DbContext.Prestamos.AsNoTracking().Where(prestamo => prestamo.Carnet == carnetUsuario);

        if (estado.HasValue)
            prestamosQuery = prestamosQuery.Where(prestamo => prestamo.EstadoPrestamo == estado.Value);

        var prestamos = await prestamosQuery.ToListAsync();
        if (!prestamos.Any()) return Result<List<PrestamoDto>>.Success(new List<PrestamoDto>());

        var prestamoIds = prestamos.Select(prestamo => prestamo.Id).ToHashSet();

        var detalles = await DbContext.DetallesPrestamos.AsNoTracking()
            .Where(detalle => prestamoIds.Contains(detalle.IdPrestamo))
            .ToListAsync();

        var usuario = await DbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(usuario => usuario.Carnet == carnetUsuario);

        var equipoIds = detalles.Select(detalle => detalle.IdEquipo).ToHashSet();

        var equipoMap = await DbContext.Equipos.AsNoTracking()
            .Include(equipo => equipo.GrupoEquipo)
            .Include(equipo => equipo.Gavetero).ThenInclude(gavetero => gavetero!.Mueble)
            .Where(equipo => equipoIds.Contains(equipo.Id))
            .ToDictionaryAsync(equipo => equipo.Id);

        var detallesByPrestamo = detalles
            .GroupBy(detalle => detalle.IdPrestamo)
            .ToDictionary(agrupacion => agrupacion.Key, agrupacion => agrupacion.ToList());

        var dtos = new List<PrestamoDto>();

        foreach (var prestamo in prestamos)
        {
            if (!detallesByPrestamo.TryGetValue(prestamo.Id, out var pDetalles))
            {
                dtos.Add(BuildDto(prestamo, usuario, null, null, null, null));
                continue;
            }

            foreach (var detalle in pDetalles)
            {
                equipoMap.TryGetValue(detalle.IdEquipo, out var equipo);
                dtos.Add(BuildDto(prestamo, usuario, equipo, equipo?.GrupoEquipo, equipo?.Gavetero, equipo?.Gavetero?.Mueble));
            }
        }

        return Result<List<PrestamoDto>>.Success(dtos);
    }

    public override async Task<Result<PrestamoDto>> Get(int id)
    {
        var prestamo = await DbContext.Prestamos.AsNoTracking().FirstOrDefaultAsync(prestamo => prestamo.Id == id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var usuario = await DbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(usuario => usuario.Carnet == prestamo.Carnet);

        var detalle = await DbContext.DetallesPrestamos.AsNoTracking().FirstOrDefaultAsync(detalle => detalle.IdPrestamo == id);
        Equipo? equipo = null;
        GrupoEquipo? grupo = null;
        Gavetero? gavetero = null;
        Mueble? mueble = null;

        if (detalle != null)
        {
            equipo = await DbContext.Equipos.AsNoTracking()
                .Include(equipo => equipo.GrupoEquipo)
                .Include(equipo => equipo.Gavetero).ThenInclude(gavetero => gavetero!.Mueble)
                .FirstOrDefaultAsync(equipo => equipo.Id == detalle.IdEquipo);

            if (equipo != null)
            {
                grupo = equipo.GrupoEquipo;
                gavetero = equipo.Gavetero;
                mueble = gavetero?.Mueble;
            }
        }

        return Result<PrestamoDto>.Success(BuildDto(prestamo, usuario, equipo, grupo, gavetero, mueble));
    }

    private static string MapEstado(EstadoPrestamo estado) => estado switch
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
            .FirstOrDefaultAsync(prestamo => prestamo.Id == id && !prestamo.EstadoEliminado);

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

