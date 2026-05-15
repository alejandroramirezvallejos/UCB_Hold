using IMT_Reservas.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.Config;

namespace IMT_Reservas.Server.Application.Features.Carrito;

public class CarritoService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CarritoService> _logger;

    public CarritoService(ApplicationDbContext dbContext, ILogger<CarritoService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<CarritoDto>>> GetDisponibilidad(CarritoDto request)
    {
        if (request.FechaInicio == null || request.FechaFin == null || request.ArrayIds == null || request.ArrayIds.Count == 0)
        {
            _logger.LogWarning("Disponibilidad request missing fields or empty IDs: Inicio={Inicio}, Fin={Fin}, IdsCount={IdsCount}", request.FechaInicio, request.FechaFin, request.ArrayIds?.Count);
            
            return Result<List<CarritoDto>>.Success(new List<CarritoDto>());
        }

        var response = new List<CarritoDto>();
        var grupos = await _dbContext.GruposEquipos
            .Where(grupoEquipo => request.ArrayIds.Contains(grupoEquipo.Id))
            .ToListAsync();
        var fechaInicio = request.FechaInicio.Value.Date;
        var fechaFin = request.FechaFin.Value.Date;

        var prestamosActivos = await (from detalle in _dbContext.DetallesPrestamos
                join prestamo in _dbContext.Prestamos on detalle.IdPrestamo equals prestamo.Id
                join equipo in _dbContext.Equipos on detalle.IdEquipo equals equipo.Id
                where request.ArrayIds.Contains(equipo.IdGrupoEquipo) &&
                      (prestamo.EstadoPrestamo == EstadoPrestamo.Activo || prestamo.EstadoPrestamo == EstadoPrestamo.Pendiente) &&
                      prestamo.FechaPrestamoEsperada.Date <= fechaFin &&
                      prestamo.FechaDevolucionEsperada.Date >= fechaInicio
                select new { equipo.IdGrupoEquipo, prestamo.FechaPrestamoEsperada, prestamo.FechaDevolucionEsperada })
            .ToListAsync();

        for (var date = fechaInicio; date <= fechaFin; date = date.AddDays(1))
        {
            foreach (var grupoId in request.ArrayIds)
            {
                var grupo = grupos.FirstOrDefault(grupoEquipo => grupoEquipo.Id == grupoId);
                
                if (grupo == null) 
                    continue;

                var totalCantidad = grupo.Cantidad;
                var ocupados = prestamosActivos.Count(prestamoActivo =>
                    prestamoActivo.IdGrupoEquipo == grupoId &&
                    prestamoActivo.FechaPrestamoEsperada.Date <= date &&
                    prestamoActivo.FechaDevolucionEsperada.Date >= date);

                response.Add(new CarritoDto
                {
                    Fecha = date,
                    IdGrupoEquipo = grupoId,
                    CantidadDisponible = Math.Max(0, totalCantidad - ocupados)
                });
            }
        }

        return Result<List<CarritoDto>>.Success(response);
    }
}
