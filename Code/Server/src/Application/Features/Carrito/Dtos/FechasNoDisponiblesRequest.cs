namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class FechasNoDisponiblesRequest
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public Dictionary<int, int>? Carrito { get; set; }
}
