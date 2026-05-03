namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;

// Create/Update Request DTO for GrupoEquipo


public class GrupoEquipoDto
{
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public string? Descripcion { get; set; }
    public string? NombreCategoria { get; set; }
    public string? UrlDataSheet { get; set; }
    public string? UrlImagen { get; set; }
}

