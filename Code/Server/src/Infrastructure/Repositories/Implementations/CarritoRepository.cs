using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarritoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CarritoRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<FechasNoDisponiblesDto>> GetUnavailableDates(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int>? carrito)
    {
        var resultado = new List<FechasNoDisponiblesDto>();

        if (carrito == null)
            return resultado;

        var diasSolicitados = (fechaFin.Date - fechaInicio.Date).Days;

        foreach (var (idGrupoEquipo, cantidadSolicitada) in carrito)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var disponibles = await GetAvailableEquipmentCount(idGrupoEquipo, fecha, diasSolicitados);

                if (disponibles < cantidadSolicitada)
                {
                    resultado.Add(new FechasNoDisponiblesDto
                    {
                        IdGrupoEquipo = idGrupoEquipo,
                        FechaNoDisponible = fecha,
                        CantidadDisponible = disponibles
                    });
                }
            }
        }
        return resultado;
    }

    public async Task<IEnumerable<DisponibilidadDto>> GetAvailability(DateTime fechaInicio, DateTime fechaFin,
        int[]? arrayIds)
    {
        var resultado = new List<DisponibilidadDto>();

        if (arrayIds == null)
            return resultado;

        var diasSolicitados = (fechaFin.Date - fechaInicio.Date).Days;

        foreach (var idGrupoEquipo in arrayIds)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var disponibles = await GetAvailableEquipmentCount(idGrupoEquipo, fecha, diasSolicitados);

                resultado.Add(new DisponibilidadDto
                {
                    Fecha = fecha,
                    IdGrupoEquipo = idGrupoEquipo,
                    CantidadDisponible = disponibles
                });
            }
        }

        return resultado;
    }

    private async Task<int> GetAvailableEquipmentCount(int idGrupoEquipo, DateTime fecha, int diasSolicitados)
    {
        var equiposOperativos = await _dbContext.Equipos
            .Where(e => e.IdGrupoEquipo == idGrupoEquipo
                && !e.EstadoEliminado
                && e.EstadoEquipo == "operativo"
                && diasSolicitados <= e.TiempoMaximoPrestamo)
            .Select(e => e.Id)
            .ToListAsync();

        var prestamosEnFecha = await _dbContext.Prestamos
            .Where(p => !p.EstadoEliminado
                && (p.EstadoPrestamo == "pendiente" || p.EstadoPrestamo == "aprobado" || p.EstadoPrestamo == "activo")
                && p.FechaPrestamoEsperada.Date <= fecha.Date
                && p.FechaDevolucionEsperada.Date >= fecha.Date)
            .Select(p => p.Id)
            .ToListAsync();

        var equiposEnPrestamo = await _dbContext.DetallesPrestamos
            .Where(dp => !dp.EstadoEliminado && prestamosEnFecha.Contains(dp.IdPrestamo))
            .Select(dp => dp.IdEquipo)
            .Distinct()
            .ToListAsync();

        var mantenimientosEnFecha = await _dbContext.Mantenimientos
            .Where(m => !m.EstadoEliminado
                && m.FechaMantenimiento.Date <= fecha.Date
                && m.FechaFinalMantenimiento.Date >= fecha.Date)
            .Select(m => m.Id)
            .ToListAsync();

        var equiposEnMantenimiento = await _dbContext.DetallesMantenimientos
            .Where(dm => mantenimientosEnFecha.Contains(dm.IdMantenimiento))
            .Select(dm => dm.IdEquipo)
            .Distinct()
            .ToListAsync();

        var equiposDisponibles = equiposOperativos
            .Except(equiposEnPrestamo)
            .Except(equiposEnMantenimiento)
            .Count();

        return equiposDisponibles;
    }
}

