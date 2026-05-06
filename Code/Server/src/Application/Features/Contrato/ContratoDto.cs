using System.Text.Json.Serialization;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoDto
{
    public int? Id { get; set; }
    public int? PrestamoId { get; set; }
    public DateTime? FechaCreacion { get; set; }
    
    [JsonIgnore]
    public IFormFile? Archivo { get; set; }
    public string? ContenidoBase64 { get; set; }
}
