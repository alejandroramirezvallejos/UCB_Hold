using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Carrito;

public class CarritoService
{
    private readonly CarritoRepository _repository;

    public CarritoService(CarritoRepository repository) => _repository = repository;

    public async Task<Result<List<CarritoDto>>> GetUnavailableDates(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int>? carrito)
    {
        var resultado = await _repository.GetUnavailableDates(fechaInicio, fechaFin, carrito);
        
        return Result<List<CarritoDto>>.Success(resultado.ToList());
    }

    public async Task<Result<List<CarritoDto>>> GetAvailability(DateTime fechaInicio, DateTime fechaFin, int[]? arrayIds)
    {
        var resultado = await _repository.GetAvailability(fechaInicio, fechaFin, arrayIds);
        
        return Result<List<CarritoDto>>.Success(resultado.ToList());
    }
}
