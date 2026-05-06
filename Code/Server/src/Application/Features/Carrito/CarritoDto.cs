namespace IMT_Reservas.Server.Application.Features.Carrito;

public class CarritoDto
{
    public DateTime? Fecha { get; set; }
    public int? IdGrupoEquipo { get; set; }
    public int? CantidadDisponible { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public List<int>? ArrayIds { get; set; } = new();
}
