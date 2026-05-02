using IMT_Reservas.Server.Application.Features.Carrito.Dtos;

namespace IMT_Reservas.Server.Infrastructure.Repositories;

public interface ICarritoRepository
{
	IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito);
	IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidadEquiposPorFechasYGrupos(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds);
	IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidad(DateTime fechaInicio, DateTime fechaFin, List<int> idsGruposEquipos);
}
