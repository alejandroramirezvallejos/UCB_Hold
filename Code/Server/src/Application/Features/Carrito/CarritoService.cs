using IMT_Reservas.Server.Core.Abstractions;
using Ardalis.Result;
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

	public Result<List<FechaNoDisponibleDto?>> ObtenerFechasNoDisponibles(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito)
	{
		var result = _repository.ObtenerFechasNoDisponibles(fechaInicio, fechaFin, carrito);
		return Result<List<FechaNoDisponibleDto?>>.Success(result.Cast<FechaNoDisponibleDto?>().ToList());
	}

	public Result<List<DisponibilidadEquipoDto?>> ObtenerDisponibilidadEquiposPorFechasYGrupos(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds)
	{
		var result = _repository.ObtenerDisponibilidadEquiposPorFechasYGrupos(fechaInicio, fechaFin, arrayIds);
		return Result<List<DisponibilidadEquipoDto?>>.Success(result.Cast<DisponibilidadEquipoDto?>().ToList());
	}
}
