using Ardalis.Result;
using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;
using IMT_Reservas.Server.Shared.Common;
using System.Text.Json;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Services.Implementations;

public class CarritoService
{
    private readonly CarritoRepository _repository;
    public CarritoService(CarritoRepository repository) => _repository = repository;

    public Result<List<FechaNoDisponibleDto>> ObtenerFechasNoDisponibles(ObtenerFechasNoDisponiblesComando input)
    {
        if (string.IsNullOrEmpty(input.Carrito))
            return Result<List<FechaNoDisponibleDto>>.Invalid(new ValidationError("Carrito", "El carrito no puede estar vacío"));

        var carritoDict = JsonSerializer.Deserialize<Dictionary<int, int>>(input.Carrito);
        if (carritoDict == null)
            return Result<List<FechaNoDisponibleDto>>.Invalid(new ValidationError("Carrito", "El carrito tiene un formato inválido"));

        var resultado = _repository.ObtenerFechasNoDisponibles(input.FechaInicio, input.FechaFin, carritoDict);
        return Result<List<FechaNoDisponibleDto>>.Success(resultado?.ToList() ?? new List<FechaNoDisponibleDto>());
    }

    public Result<List<DisponibilidadEquipoDto>> ObtenerDisponibilidadEquiposPorFechasYGrupos(ObtenerDisponibilidadEquiposComando input)
    {
        if (input == null)
            return Result<List<DisponibilidadEquipoDto>>.Invalid(new ValidationError("input", "El comando no puede ser nulo"));

        if (input.ArrayIds == null || input.ArrayIds.Length == 0)
            return Result<List<DisponibilidadEquipoDto>>.Invalid(new ValidationError("ArrayIds", "El array de IDs no puede estar vacío"));

        if (input.FechaFin < input.FechaInicio)
            return Result<List<DisponibilidadEquipoDto>>.Invalid(new ValidationError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio"));

        var resultado = _repository.ObtenerDisponibilidadEquiposPorFechasYGrupos(input.FechaInicio, input.FechaFin, input.ArrayIds);
        return Result<List<DisponibilidadEquipoDto>>.Success(resultado?.ToList() ?? new List<DisponibilidadEquipoDto>());
    }
}
