namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class GetAvailabilityRequest
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int[]? ArrayIds { get; set; }
}

public class GetUnavailableDatesRequest
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public Dictionary<int, int>? Carrito { get; set; }
}
