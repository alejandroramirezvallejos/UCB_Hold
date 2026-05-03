namespace IMT_Reservas.Server.Application.Features.Accesorio.Dtos;

// Create/Update Request DTO for Accesorio


public class AccesorioDto
{
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? CodigoImt { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Precio { get; set; }
    public string? UrlDataSheet { get; set; }
}

