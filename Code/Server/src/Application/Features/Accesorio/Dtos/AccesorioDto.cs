namespace IMT_Reservas.Server.Application.Features.Accesorio.Dtos;

public class AccesorioDto
{
    public int? Id { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Precio { get; set; }
    public string? UrlDataSheet { get; set; }
    public string? CodigoImt { get; set; }
    public string? NombreEquipoAsociado { get; set; }
}
