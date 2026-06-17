using System.Text.RegularExpressions;
using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Prestamo.State;
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

    public override async Task<Result<List<PrestamoDto>>> GetAll()
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

    public async Task<Result<List<PrestamoDto>>> GetHistoryWithDetails(string carnetUsuario, EstadoPrestamo? estado)
    {
        var query = DbContext.Prestamos.AsNoTracking().Where(p => p.Carnet == carnetUsuario);

        if (estado.HasValue)
            query = query.Where(p => p.EstadoPrestamo == estado.Value);

        return Result<List<PrestamoDto>>.Success(await GetPrestamoList(query));
    }

    protected override async Task CascadeDelete(PrestamoEntity prestamo)
        => await CascadeLeaf<DetallePrestamo>(d => d.IdPrestamo == prestamo.Id);

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

    public async Task<List<(int Codigo, string? Nombre, string Estado)>> AplicarEstadosRetorno(
        int prestamoId, Dictionary<int, EstadoEquipo> estadosPorCodigo)
    {
        var detalles = await DbContext.DetallesPrestamos
            .Where(d => d.IdPrestamo == prestamoId && d.IdEquipo != null)
            .ToListAsync();

        var equipoIds = detalles.Select(d => d.IdEquipo!.Value).ToList();

        var equipos = await DbContext.Equipos
            .Where(e => equipoIds.Contains(e.Id))
            .ToListAsync();

        var grupos = await DbContext.GruposEquipos
            .Where(g => equipos.Select(e => e.IdGrupoEquipo).Contains(g.Id))
            .ToDictionaryAsync(g => g.Id, g => g.Nombre);

        var resultado = new List<(int, string?, string)>();

        foreach (var equipo in equipos)
        {
            if (!estadosPorCodigo.TryGetValue(equipo.CodigoImt, out var estado))
                continue;

            equipo.EstadoEquipo = estado;

            var detalle = detalles.FirstOrDefault(d => d.IdEquipo == equipo.Id);
            
            if (detalle != null)
                detalle.EstadoEquipoRetorno = estado;

            grupos.TryGetValue(equipo.IdGrupoEquipo, out var nombre);
            resultado.Add((equipo.CodigoImt, nombre, EstadoEquipoToPg(estado)));
        }

        await DbContext.SaveChangesAsync();
        
        return resultado;
    }

    private static string EstadoEquipoToPg(EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.ParcialmenteOperativo => "parcialmente_operativo",
        EstadoEquipo.Inoperativo           => "inoperativo",
        _                                  => "operativo"
    };

    public async Task<bool> HasAvailableEquipo(int grupoId, DateTime fechaInicio, DateTime fechaFin)
    {
        return await DbContext.Equipos
            .Where(e => e.IdGrupoEquipo == grupoId
                && !e.EstadoEliminado
                && e.EstadoEquipo == EstadoEquipo.Operativo
                && !DbContext.DetallesPrestamos
                    .Join(DbContext.Prestamos, d => d.IdPrestamo, p => p.Id, (d, p) => new { d, p })
                    .Any(x => x.d.IdEquipo == e.Id
                        && (x.p.EstadoPrestamo == EstadoPrestamo.Aprobado
                         || x.p.EstadoPrestamo == EstadoPrestamo.Activo
                         || x.p.EstadoPrestamo == EstadoPrestamo.Atrasado)
                        && x.p.FechaPrestamoEsperada.Date <= fechaFin.Date
                        && x.p.FechaDevolucionEsperada.Date >= fechaInicio.Date))
            .AnyAsync();
    }

    public async Task SaveGrupoReservas(int prestamoId, List<int>? grupoEquipoIds)
    {
        if (grupoEquipoIds == null || grupoEquipoIds.Count == 0)
            return;

        foreach (var groupId in grupoEquipoIds)
        {
            DbContext.DetallesPrestamos.Add(new DetallePrestamo
            {
                IdPrestamo = prestamoId,
                IdGrupoEquipo = groupId,
                IdEquipo = null,
                EstadoEliminado = false
            });
        }

        await DbContext.SaveChangesAsync();
    }

    public async Task<bool> AssignEquiposOnApproval(int prestamoId)
    {
        var prestamo = await DbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == prestamoId);
       
        if (prestamo == null) 
            return false;

        var detalles = await DbContext.DetallesPrestamos
            .Where(d => d.IdPrestamo == prestamoId && !d.EstadoEliminado && d.IdEquipo == null)
            .ToListAsync();

        if (detalles.Count == 0) 
            return true;

        var loanedIds = await DbContext.DetallesPrestamos
            .Join(DbContext.Prestamos, d => d.IdPrestamo, p => p.Id, (d, p) => new { d, p })
            .Where(x => x.d.IdEquipo != null
                      && (x.p.EstadoPrestamo == EstadoPrestamo.Aprobado
                       || x.p.EstadoPrestamo == EstadoPrestamo.Activo
                       || x.p.EstadoPrestamo == EstadoPrestamo.Atrasado)
                      && x.p.FechaPrestamoEsperada.Date <= prestamo.FechaDevolucionEsperada.Date
                      && x.p.FechaDevolucionEsperada.Date >= prestamo.FechaPrestamoEsperada.Date)
            .Select(x => x.d.IdEquipo!.Value)
            .ToListAsync();

        var assignedIds = new List<int>();

        foreach (var detalle in detalles)
        {
            var excluded = loanedIds.Concat(assignedIds).ToList();

            var equipo = await DbContext.Equipos
                .FirstOrDefaultAsync(e => e.IdGrupoEquipo == detalle.IdGrupoEquipo
                    && !e.EstadoEliminado
                    && e.EstadoEquipo == EstadoEquipo.Operativo
                    && !excluded.Contains(e.Id));

            if (equipo == null)
                return false;

            detalle.IdEquipo = equipo.Id;
            assignedIds.Add(equipo.Id);
        }

        await DbContext.SaveChangesAsync();
        return true;
    }

    public async Task UpdateContratoWithEquipos(int prestamoId)
    {
        var prestamo = await DbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == prestamoId);
        
        if (prestamo?.IdContrato == null) 
            return;

        var contrato = await DbContext.Contratos.FirstOrDefaultAsync(c => c.Id == prestamo.IdContrato);
        
        if (contrato == null || string.IsNullOrEmpty(contrato.ContratoHtml)) 
            return;

        var detalles = await DbContext.DetallesPrestamos
            .Where(d => d.IdPrestamo == prestamoId && !d.EstadoEliminado && d.IdEquipo != null)
            .ToListAsync();

        var equiposByGrupo = new Dictionary<int, List<Equipo>>();

        foreach (var detalle in detalles)
        {
            var equipo = await DbContext.Equipos.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == detalle.IdEquipo);

            if (equipo == null) 
                continue;

            if (!equiposByGrupo.ContainsKey(detalle.IdGrupoEquipo))
                equiposByGrupo[detalle.IdGrupoEquipo] = new List<Equipo>();
                
            equiposByGrupo[detalle.IdGrupoEquipo].Add(equipo);
        }

        var html = contrato.ContratoHtml;

        foreach (var (grupoId, equipos) in equiposByGrupo)
        {
            var imtCodes = string.Join(", ", equipos.Select(e => e.CodigoImt.ToString()));
            var ucbCodes = string.Join(", ", equipos.Select(e => e.CodigoUcb ?? "-"));
            var serials = string.Join(", ", equipos.Select(e => e.NumeroSerial ?? "-"));

            html = Regex.Replace(html,
                $@"<td[^>]*class=""imt-code""[^>]*data-grupo-id=""{grupoId}""[^>]*>.*?</td>",
                $@"<td class=""imt-code"" data-grupo-id=""{grupoId}"">{imtCodes}</td>",
                RegexOptions.None, TimeSpan.FromMilliseconds(500));

            html = Regex.Replace(html,
                $@"<td[^>]*class=""ucb-code""[^>]*data-grupo-id=""{grupoId}""[^>]*>.*?</td>",
                $@"<td class=""ucb-code"" data-grupo-id=""{grupoId}"">{ucbCodes}</td>",
                RegexOptions.None, TimeSpan.FromMilliseconds(500));

            html = Regex.Replace(html,
                $@"<td[^>]*class=""serial-code""[^>]*data-grupo-id=""{grupoId}""[^>]*>.*?</td>",
                $@"<td class=""serial-code"" data-grupo-id=""{grupoId}"">{serials}</td>",
                RegexOptions.None, TimeSpan.FromMilliseconds(500));
        }

        contrato.ContratoHtml = html;
        DbContext.Contratos.Update(contrato);
        await DbContext.SaveChangesAsync();
    }

    public async Task<string?> GetGrupoEquipoNombre(int grupoId)
        => await DbContext.GruposEquipos.AsNoTracking()
            .Where(g => g.Id == grupoId && !g.EstadoEliminado)
            .Select(g => g.Nombre)
            .FirstOrDefaultAsync();

    public async Task SaveContrato(PrestamoEntity entity, string? contratoHtml)
    {
        if (string.IsNullOrEmpty(contratoHtml))
            return;

        var contrato = new Contrato { ContratoHtml = contratoHtml };
        DbContext.Contratos.Add(contrato);
        await DbContext.SaveChangesAsync();

        entity.IdContrato = contrato.Id;
        DbContext.Prestamos.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task<bool> HasAtrasadoPrestamo(string carnet)
        => await DbContext.Prestamos
            .AnyAsync(p => p.Carnet == carnet
                        && p.EstadoPrestamo == EstadoPrestamo.Atrasado
                        && !p.EstadoEliminado);

    private async Task<List<PrestamoDto>> GetPrestamoList(IQueryable<PrestamoEntity> source)
    {
        var rows = await (
            from prestamo in source.OrderByDescending(p => p.FechaSolicitud)
            join usuario in DbContext.Usuarios.AsNoTracking().IgnoreQueryFilters()
                on prestamo.Carnet equals usuario.Carnet into usuarioJoin
            from usuario in usuarioJoin.DefaultIfEmpty()
            join detalle in DbContext.DetallesPrestamos.AsNoTracking().IgnoreQueryFilters().Where(d => !d.EstadoEliminado)
                on prestamo.Id equals detalle.IdPrestamo into detalleJoin
            from detalle in detalleJoin.DefaultIfEmpty()
            join equipo in DbContext.Equipos.AsNoTracking().IgnoreQueryFilters()
                on detalle.IdEquipo equals equipo.Id into equipoJoin
            from equipo in equipoJoin.DefaultIfEmpty()
            join grupoReserva in DbContext.GruposEquipos.AsNoTracking().IgnoreQueryFilters()
                on detalle.IdGrupoEquipo equals grupoReserva.Id into grupoReservaJoin
            from grupoReserva in grupoReservaJoin.DefaultIfEmpty()
            join gavetero in DbContext.Gaveteros.AsNoTracking().IgnoreQueryFilters()
                on equipo.IdGavetero equals gavetero.Id into gaveteroJoin
            from gavetero in gaveteroJoin.DefaultIfEmpty()
            join mueble in DbContext.Muebles.AsNoTracking().IgnoreQueryFilters()
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
                NombreGrupoEquipo = grupoReserva != null ? grupoReserva.Nombre : null,
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
