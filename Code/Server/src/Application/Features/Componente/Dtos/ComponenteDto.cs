namespace IMT_Reservas.Server.Application.Features.Componente.Dtos;

public class ComponenteDto
{
    public int? Id { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public string? NombreEquipo { get; set; }
    public string? CodigoImtEquipo { get; set; }
    public string? UrlDataSheet { get; set; }
}
