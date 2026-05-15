using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarritoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CarritoRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Dictionary<int, int>> GetCantidadesByGrupos(List<int> grupoIds)
        => await _dbContext.GruposEquipos
            .Where(g => grupoIds.Contains(g.Id))
            .Select(g => new { g.Id, g.Cantidad })
            .ToDictionaryAsync(g => g.Id, g => g.Cantidad);

    public async Task<List<(int IdGrupoEquipo, DateTime FechaPrestamo, DateTime FechaDevolucion)>> GetPrestamosActivosEnRango(
        List<int> grupoIds, DateTime fechaInicio, DateTime fechaFin)
    {
        var rows = await (
            from detalle in _dbContext.DetallesPrestamos
            join prestamo in _dbContext.Prestamos on detalle.IdPrestamo equals prestamo.Id
            join equipo   in _dbContext.Equipos   on detalle.IdEquipo    equals equipo.Id
            where grupoIds.Contains(equipo.IdGrupoEquipo)
               && (prestamo.EstadoPrestamo == EstadoPrestamo.Activo
                   || prestamo.EstadoPrestamo == EstadoPrestamo.Pendiente)
               && prestamo.FechaPrestamoEsperada.Date <= fechaFin
               && prestamo.FechaDevolucionEsperada.Date >= fechaInicio
            select new { equipo.IdGrupoEquipo, prestamo.FechaPrestamoEsperada, prestamo.FechaDevolucionEsperada })
            .ToListAsync();

        return rows.ConvertAll(r => (r.IdGrupoEquipo, r.FechaPrestamoEsperada, r.FechaDevolucionEsperada));
    }
}
