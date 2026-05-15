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
    public PrestamoRepository(ApplicationDbContext dbContext, PrestamoMapper mapper)
        : base(dbContext, mapper) { }

    public override async Task<Result<List<PrestamoDto>>> GetAll(QueryFilter? filter = null)
    {
        var list = await GetPrestamoList(DbContext.Prestamos.AsNoTracking());
        return Result<List<PrestamoDto>>.Success(list);
    }

    public override async Task<Result<PrestamoDto>> Get(int id)
    {
        var list = await GetPrestamoList(DbContext.Prestamos.AsNoTracking().Where(p => p.Id == id));
        var item = list.FirstOrDefault();
        return item == null ? Result<PrestamoDto>.NotFound() : Result<PrestamoDto>.Success(item);
    }

    public async Task<Result<List<PrestamoDto>>> GetHistorialWithDetalles(string carnetUsuario, EstadoPrestamo? estado)
    {
        var query = DbContext.Prestamos.AsNoTracking().Where(p => p.Carnet == carnetUsuario);

        if (estado.HasValue)
            query = query.Where(p => p.EstadoPrestamo == estado.Value);

        return Result<List<PrestamoDto>>.Success(await GetPrestamoList(query));
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Prestamos
            .FirstOrDefaultAsync(p => p.Id == id && !p.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        var detalles = await DbContext.DetallesPrestamos
            .Where(d => d.IdPrestamo == id && !d.EstadoEliminado)
            .ToListAsync();

        foreach (var detalle in detalles)
            detalle.EstadoEliminado = true;

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }

    public async Task<PrestamoEntity?> FindById(int id)
        => await DbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == id);

    public async Task SavePrestamo(PrestamoEntity entity)
    {
        DbContext.Prestamos.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateTracked(PrestamoEntity entity)
    {
        DbContext.Prestamos.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task AssignEquipos(int prestamoId, List<int>? grupoEquipoIds)
    {
        if (grupoEquipoIds == null || !grupoEquipoIds.Any())
            return;

        var assignedEquipoIds = new List<int>();

        foreach (var groupId in grupoEquipoIds)
        {
            var loanedIds = await DbContext.DetallesPrestamos
                .Join(DbContext.Prestamos, d => d.IdPrestamo, p => p.Id, (d, p) => new { d, p })
                .Where(x => x.p.EstadoPrestamo == EstadoPrestamo.Pendiente
                          || x.p.EstadoPrestamo == EstadoPrestamo.Aprobado
                          || x.p.EstadoPrestamo == EstadoPrestamo.Activo)
                .Select(x => x.d.IdEquipo)
                .ToListAsync();

            loanedIds.AddRange(assignedEquipoIds);

            var equipoDisponible = await DbContext.Equipos
                .FirstOrDefaultAsync(e => e.IdGrupoEquipo == groupId
                    && !e.EstadoEliminado
                    && e.EstadoEquipo == EstadoEquipo.Operativo
                    && !loanedIds.Contains(e.Id));

            if (equipoDisponible == null)
                continue;

            DbContext.DetallesPrestamos.Add(new DetallePrestamo
            {
                IdPrestamo = prestamoId,
                IdEquipo = equipoDisponible.Id,
                EstadoEliminado = false
            });
            assignedEquipoIds.Add(equipoDisponible.Id);
        }

        await DbContext.SaveChangesAsync();
    }

    public async Task SaveContrato(PrestamoEntity entity, string? contratoHtml)
    {
        if (string.IsNullOrEmpty(contratoHtml))
            return;

        var contrato = new Core.Entities.Contrato { ContratoHtml = contratoHtml };
        DbContext.Contratos.Add(contrato);
        await DbContext.SaveChangesAsync();

        entity.IdContrato = contrato.Id;
        DbContext.Prestamos.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    private async Task<List<PrestamoDto>> GetPrestamoList(IQueryable<PrestamoEntity> source)
    {
        var rows = await (
            from prestamo in source
            join usuario in DbContext.Usuarios.AsNoTracking()
                on prestamo.Carnet equals usuario.Carnet into usuarioJoin
            from usuario in usuarioJoin.DefaultIfEmpty()
            join detalle in DbContext.DetallesPrestamos.AsNoTracking().Where(d => !d.EstadoEliminado)
                on prestamo.Id equals detalle.IdPrestamo into detalleJoin
            from detalle in detalleJoin.DefaultIfEmpty()
            join equipo in DbContext.Equipos.AsNoTracking()
                on detalle.IdEquipo equals equipo.Id into equipoJoin
            from equipo in equipoJoin.DefaultIfEmpty()
            join grupo in DbContext.GruposEquipos.AsNoTracking()
                on equipo.IdGrupoEquipo equals grupo.Id into grupoJoin
            from grupo in grupoJoin.DefaultIfEmpty()
            join gavetero in DbContext.Gaveteros.AsNoTracking()
                on equipo.IdGavetero equals gavetero.Id into gaveteroJoin
            from gavetero in gaveteroJoin.DefaultIfEmpty()
            join mueble in DbContext.Muebles.AsNoTracking()
                on gavetero.IdMueble equals mueble.Id into muebleJoin
            from mueble in muebleJoin.DefaultIfEmpty()
            select new
            {
                PrestamoId = prestamo.Id,
                prestamo.Carnet,
                UsuarioNombre = usuario != null ? usuario.Nombre : null,
                UsuarioApellido = usuario != null ? usuario.ApellidoPaterno : null,
                UsuarioTelefono = usuario != null ? usuario.Telefono : null,
                prestamo.EstadoPrestamo,
                prestamo.FechaSolicitud,
                prestamo.FechaPrestamoEsperada,
                prestamo.FechaPrestamo,
                prestamo.FechaDevolucionEsperada,
                prestamo.FechaDevolucion,
                prestamo.Observacion,
                prestamo.IdContrato,
                NombreGrupoEquipo = grupo != null ? grupo.Nombre : null,
                CodigoImt = equipo != null ? (int?)equipo.CodigoImt : null,
                UbicacionEquipo = equipo != null ? equipo.Ubicacion : null,
                NombreGavetero = gavetero != null ? gavetero.Nombre : null,
                NombreMueble = mueble != null ? mueble.Nombre : null,
                UbicacionMueble = mueble != null ? mueble.Ubicacion : null
            }
        ).ToListAsync();

        return rows.Select(r => new PrestamoDto
        {
            Id = r.PrestamoId,
            CarnetUsuario = r.Carnet,
            NombreUsuario = r.UsuarioNombre,
            ApellidoPaternoUsuario = r.UsuarioApellido,
            TelefonoUsuario = r.UsuarioTelefono,
            EstadoPrestamo = PrestamoState.ToText(r.EstadoPrestamo),
            FechaSolicitud = r.FechaSolicitud,
            FechaPrestamoEsperada = r.FechaPrestamoEsperada,
            FechaPrestamo = r.FechaPrestamo,
            FechaDevolucionEsperada = r.FechaDevolucionEsperada,
            FechaDevolucion = r.FechaDevolucion,
            Observacion = r.Observacion,
            IdContrato = r.IdContrato,
            NombreGrupoEquipo = r.NombreGrupoEquipo,
            CodigoImt = r.CodigoImt?.ToString(),
            UbicacionEquipo = r.UbicacionEquipo,
            NombreGavetero = r.NombreGavetero,
            NombreMueble = r.NombreMueble,
            UbicacionMueble = r.UbicacionMueble
        }).ToList();
    }
}
