namespace IMT_Reservas.Server.Application.Dtos;

public class PrestamoDto
{
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? Observacion { get; set; }
    public string? Carnet { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
}
