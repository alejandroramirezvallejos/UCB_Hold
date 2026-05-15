using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Carrito;

public class CarritoService
{
    private readonly CarritoRepository _repository;
    private readonly ILogger<CarritoService> _logger;

    public CarritoService(CarritoRepository repository, ILogger<CarritoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<CarritoDto>>> GetDisponibilidad(CarritoDto request)
    {
        if (request.FechaInicio == null || request.FechaFin == null
            || request.ArrayIds == null || request.ArrayIds.Count == 0)
        {
            _logger.LogWarning(
                "Disponibilidad request missing fields or empty IDs: Inicio={Inicio}, Fin={Fin}, IdsCount={IdsCount}",
                request.FechaInicio, request.FechaFin, request.ArrayIds?.Count);

            return Result<List<CarritoDto>>.Success([]);
        }

        var fechaInicio = request.FechaInicio.Value.Date;
        var fechaFin    = request.FechaFin.Value.Date;

        var cantidades       = await _repository.GetCantidadesByGrupos(request.ArrayIds);
        var prestamosActivos = await _repository.GetPrestamosActivosEnRango(
            request.ArrayIds, fechaInicio, fechaFin);

        var response = new List<CarritoDto>();

        for (var date = fechaInicio; date <= fechaFin; date = date.AddDays(1))
        {
            foreach (var grupoId in request.ArrayIds)
            {
                if (!cantidades.TryGetValue(grupoId, out var total))
                    continue;

                var ocupados = prestamosActivos.Count(p =>
                    p.IdGrupoEquipo == grupoId
                    && p.FechaPrestamo.Date <= date
                    && p.FechaDevolucion.Date >= date);

                response.Add(new CarritoDto
                {
                    Fecha = date,
                    IdGrupoEquipo = grupoId,
                    CantidadDisponible = Math.Max(0, total - ocupados)
                });
            }
        }

        return Result<List<CarritoDto>>.Success(response);
    }
}
