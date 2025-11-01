using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;

namespace IMT_Reservas.Server.Application.Services.Interfaces;

public interface ICarritoService
{
    IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(ObtenerFechasNoDisponiblesComando input);
    IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidadEquiposPorFechasYGrupos(ObtenerDisponibilidadEquiposComando input);
}

