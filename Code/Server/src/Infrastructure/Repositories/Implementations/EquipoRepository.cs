using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoEntity, EquipoDto>
{
    public EquipoRepository(ApplicationDbContext dbContext, EquipoMapper mapper)
        : base(dbContext, mapper) { }

    public async Task<int> GetMaxCodigoImt()
        => await DbContext.Equipos.MaxAsync(e => (int?)e.CodigoImt) ?? 0;

    public async Task<bool> ExistsByCodigoImt(int codigoImt, int? excludeId = null)
        => await DbContext.Equipos
            .AnyAsync(e => e.CodigoImt == codigoImt && !e.EstadoEliminado && e.Id != excludeId);

    public async Task<EquipoEntity?> FindById(int id)
        => await DbContext.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.EstadoEliminado);

    public async Task RecalcGrupoStats(int idGrupoEquipo)
    {
        var grupo = await DbContext.GruposEquipos
            .FirstOrDefaultAsync(g => g.Id == idGrupoEquipo);

        if (grupo == null) return;

        var stats = await DbContext.Equipos
            .Where(e => e.IdGrupoEquipo == idGrupoEquipo && !e.EstadoEliminado)
            .Select(e => new { e.CostoReferencia })
            .ToListAsync();

        grupo.Cantidad = stats.Count;
        grupo.CostoPromedio = stats.Count == 0
            ? 0
            : (decimal)(stats.Where(e => e.CostoReferencia.HasValue)
                .Sum(e => e.CostoReferencia ?? 0) / Math.Max(1, stats.Count));

        await DbContext.SaveChangesAsync();
    }

    public async Task<List<EquipoDto>> GetByGrupo(int grupoId)
        => await ProjectTo(DbContext.Equipos
            .Where(e => e.IdGrupoEquipo == grupoId && !e.EstadoEliminado))
            .ToListAsync();

    public async Task<List<EquipoDto>> GetByGavetero(int gaveteroId)
        => await ProjectTo(DbContext.Equipos
            .Where(e => e.IdGavetero == gaveteroId && !e.EstadoEliminado))
            .ToListAsync();

    public async Task<List<HistorialEquipoDto>> GetHistorial(int equipoId)
    {
        var rows = await DbContext.DetallesPrestamos
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(d => d.IdEquipo == equipoId)
            .Join(DbContext.Prestamos.IgnoreQueryFilters(),
                d => d.IdPrestamo,
                p => p.Id,
                (d, p) => new { d, p })
            .Join(DbContext.Usuarios.IgnoreQueryFilters(),
                x => x.p.Carnet,
                u => u.Carnet,
                (x, u) => new
                {
                    x.p.Id,
                    u.Carnet,
                    u.Nombre,
                    u.ApellidoPaterno,
                    x.p.FechaPrestamo,
                    x.p.FechaDevolucionEsperada,
                    x.p.FechaDevolucion,
                    x.p.EstadoPrestamo,
                    x.d.EstadoEquipoRetorno,
                    x.p.Observacion
                })
            .OrderByDescending(r => r.FechaPrestamo)
            .ToListAsync();

        return rows.Select(r => new HistorialEquipoDto
        {
            IdPrestamo              = r.Id,
            Carnet                  = r.Carnet,
            NombreUsuario           = r.Nombre + " " + r.ApellidoPaterno,
            FechaPrestamo           = r.FechaPrestamo,
            FechaDevolucionEsperada = r.FechaDevolucionEsperada,
            FechaDevolucion         = r.FechaDevolucion,
            EstadoPrestamo          = r.EstadoPrestamo.ToString().ToLower(),
            EstadoEquipo            = r.EstadoEquipoRetorno.HasValue ? EstadoEquipoToPg(r.EstadoEquipoRetorno.Value) : null,
            Observacion             = r.Observacion
        }).ToList();
    }

    private static string EstadoEquipoToPg(EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.ParcialmenteOperativo => "parcialmente_operativo",
        EstadoEquipo.Inoperativo           => "inoperativo",
        _                                  => "operativo"
    };

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Equipos
            .FirstOrDefaultAsync(equipo => equipo.Id == id && !equipo.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }
}
