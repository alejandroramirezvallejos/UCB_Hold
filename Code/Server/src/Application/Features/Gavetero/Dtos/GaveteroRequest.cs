namespace IMT_Reservas.Server.Application.Features.Gavetero.Dtos;

// Create/Update Request DTO for Gavetero


public class GaveteroDto
{
    public string? Nombre { get; set; }
    public string? Tipo { get; set; }
    public string? NombreMueble { get; set; }
    public decimal? Longitud { get; set; }
    public decimal? Profundidad { get; set; }
    public decimal? Altura { get; set; }
}

