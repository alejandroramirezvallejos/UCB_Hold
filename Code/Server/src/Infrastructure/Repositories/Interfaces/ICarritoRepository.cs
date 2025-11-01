using System.Data;
using IMT_Reservas.Server.Application.ResponseDTOs;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Interfaces;

public interface ICarritoRepository
{
    IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito);
    IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidadEquiposPorFechasYGrupos(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds);
}