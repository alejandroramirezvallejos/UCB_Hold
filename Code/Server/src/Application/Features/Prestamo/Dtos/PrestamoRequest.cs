using Microsoft.AspNetCore.Http;

namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class CreatePrestamoRequest
{
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? Observacion { get; set; }
    public string? Carnet { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
    public int[]? EquipoIds { get; set; }
    public IFormFile? Contrato { get; set; }
}

public class PrestamoDto
{
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? Observacion { get; set; }
    public string? Carnet { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
}
