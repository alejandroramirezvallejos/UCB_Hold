using System.Text.Json.Serialization;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoDto
{
    public int? Id { get; set; }
    public int? PrestamoId { get; set; }
    [JsonIgnore]
    public IFormFile? Archivo { get; set; }
    public string? ContratoHtml { get; set; }
}
