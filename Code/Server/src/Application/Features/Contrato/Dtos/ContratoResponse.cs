namespace IMT_Reservas.Server.Application.Features.Contrato.Dtos;

public class ContratoResponse
{
    public string Id { get; set; } = string.Empty;
    public int PrestamoId { get; set; }
    public string? FileId { get; set; }
    public DateTime FechaCreacion { get; set; }
}
