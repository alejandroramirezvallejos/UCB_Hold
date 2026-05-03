namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using Microsoft.AspNetCore.Http;

public class PrestamoRequest
{
    public DateTime? FechaPrestamoEsperada { get; set; }
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? Observacion { get; set; }
    public int[]? EquipoIds { get; set; }
    public IFormFile? Contrato { get; set; }
}
