namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class CreatePrestamoRequest
{
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? Observacion { get; set; }
    public string? Carnet { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
}
