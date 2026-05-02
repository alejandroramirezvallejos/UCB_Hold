using Ardalis.Result;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Application.Features.Carrito.Dtos;

namespace IMT_Reservas.Server.Application.Features.Carrito;

public class CarritoService
{
    private readonly CarritoRepository _repository;

    public CarritoService(CarritoRepository repository)
    {
        _repository = repository;
    }

    public Result<List<FechaNoDisponibleDto?>> GetUnavailableDates(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito)
    {
        var result = _repository.GetUnavailableDates(fechaInicio, fechaFin, carrito);
        return Result<List<FechaNoDisponibleDto?>>.Success(result.Cast<FechaNoDisponibleDto?>().ToList());
    }

    public Result<List<DisponibilidadEquipoDto?>> GetAvailability(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds)
    {
        var result = _repository.GetAvailability(fechaInicio, fechaFin, arrayIds);
        return Result<List<DisponibilidadEquipoDto?>>.Success(result.Cast<DisponibilidadEquipoDto?>().ToList());
    }
}
