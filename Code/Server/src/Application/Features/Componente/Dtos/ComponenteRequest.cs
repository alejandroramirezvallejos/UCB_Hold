namespace IMT_Reservas.Server.Application.Features.Componente.Dtos;

// Create/Update Request DTO for Componente


public class ComponenteDto
{
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? CodigoImt { get; set; }
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public string? UrlDataSheet { get; set; }
}
