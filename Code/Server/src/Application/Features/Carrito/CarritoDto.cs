namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class CarritoDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public Dictionary<int, int>? Carrito { get; set; }
    public int[]? ArrayIds { get; set; }
    public int IdGrupoEquipo { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime? FechaNoDisponible { get; set; }
    public int CantidadDisponible { get; set; }
}
