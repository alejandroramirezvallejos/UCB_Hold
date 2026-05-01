namespace IMT_Reservas.Server.Application.DTOs.Response;

public class GrupoEquipoDto : Dto
{
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public string? NombreCategoria { get; set; }
    public int? Cantidad { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlDataSheet { get; set; }
    public string? UrlImagen { get; set; }
    public decimal? CostoPromedio { get; set; }
}
