namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class DisponibilidadRequest
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int[]? ArrayIds { get; set; }
}
