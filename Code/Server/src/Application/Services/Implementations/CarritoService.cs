using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;
using IMT_Reservas.Server.Application.Services.Interfaces;
using IMT_Reservas.Server.Infrastructure.Repositories.Interfaces;
using IMT_Reservas.Server.Shared.Common;
using System.Text.Json;

namespace IMT_Reservas.Server.Application.Services.Implementations;

public class CarritoService : ICarritoService
{
    private readonly ICarritoRepository _repository;
    public CarritoService(ICarritoRepository repository) => _repository = repository;

    public IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(ObtenerFechasNoDisponiblesComando input)
    {
        if (string.IsNullOrEmpty(input.Carrito))
            throw new ArgumentException("El carrito no puede estar vacío.");

        var carritoDict = JsonSerializer.Deserialize<Dictionary<int, int>>(input.Carrito)
            ?? throw new ArgumentException("El carrito tiene un formato inválido.");

        return _repository.ObtenerFechasNoDisponibles(input.FechaInicio, input.FechaFin, carritoDict);
    }

    public IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidadEquiposPorFechasYGrupos(ObtenerDisponibilidadEquiposComando input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input), "El comando no puede ser nulo.");

        if (input.ArrayIds == null || input.ArrayIds.Length == 0)
            throw new ArgumentException("El array de IDs no puede estar vacío.");

        if (input.FechaFin < input.FechaInicio)
            throw new ArgumentException("La fecha de fin no puede ser menor a la fecha de inicio.");

        return _repository.ObtenerDisponibilidadEquiposPorFechasYGrupos(input.FechaInicio, input.FechaFin, input.ArrayIds);
    }
}
