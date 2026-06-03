using IMT_Reservas.Server.Core.Abstraction;
namespace IMT_Reservas.Server.Core.Entities;

public class Prestamo : Entity
{
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaPrestamoEsperada { get; set; }
    public DateTime? FechaPrestamo { get; set; }
    public DateTime FechaDevolucionEsperada { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? Carnet { get; set; } = string.Empty;
    public EstadoPrestamo EstadoPrestamo { get; set; }
    public string? Observacion { get; set; }
    public int? IdContrato { get; set; }
}
