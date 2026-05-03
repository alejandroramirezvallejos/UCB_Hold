namespace IMT_Reservas.Server.Application.Features.Contrato.Dtos;

public class ContratoDetail
{
    public string? Id { get; set; }
    public int PrestamoId { get; set; }
    public string? FileId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
