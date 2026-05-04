namespace IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using System.Text.Json.Serialization;

public class ContratoDto
{
    public int? Id { get; set; }
    public int? PrestamoId { get; set; }
    public DateTime? FechaCreacion { get; set; }
    
    [JsonIgnore]
    public IFormFile? Archivo { get; set; }
}
