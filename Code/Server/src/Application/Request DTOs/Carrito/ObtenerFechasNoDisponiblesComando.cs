using System.Text.Json.Serialization;

namespace IMT_Reservas.Server.Application.RequestDTOs.Carrito;

public class ObtenerFechasNoDisponiblesComando
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string? Carrito { get; set; } // Cambiado a string nullable para [FromQuery]
}
