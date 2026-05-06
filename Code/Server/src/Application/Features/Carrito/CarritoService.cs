using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;
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
        var grupos = await _dbContext.GruposEquipos.Where(g => request.ArrayIds.Contains(g.Id)).ToListAsync();
        var fechaInicio = request.FechaInicio.Value.Date;
        var fechaFin = request.FechaFin.Value.Date;

        var prestamosActivos = await (from dp in _dbContext.DetallesPrestamos
                join p in _dbContext.Prestamos on dp.IdPrestamo equals p.Id
                join e in _dbContext.Equipos on dp.IdEquipo equals e.Id
                where request.ArrayIds.Contains(e.IdGrupoEquipo) &&
                      (p.EstadoPrestamo == EstadoPrestamo.Activo || p.EstadoPrestamo == EstadoPrestamo.Pendiente) &&
                      p.FechaPrestamoEsperada.Date <= fechaFin &&
                      p.FechaDevolucionEsperada.Date >= fechaInicio
                select new { e.IdGrupoEquipo, p.FechaPrestamoEsperada, p.FechaDevolucionEsperada })
            .ToListAsync();

        for (var date = fechaInicio; date <= fechaFin; date = date.AddDays(1))
        {
            foreach (var grupoId in request.ArrayIds)
            {
                var grupo = grupos.FirstOrDefault(g => g.Id == grupoId);
                
                if (grupo == null) 
                    continue;

                var totalCantidad = grupo.Cantidad;
                var ocupados = prestamosActivos.Count(pa =>
                    pa.IdGrupoEquipo == grupoId &&
                    pa.FechaPrestamoEsperada.Date <= date &&
                    pa.FechaDevolucionEsperada.Date >= date);

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
