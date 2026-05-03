namespace IMT_Reservas.Server.Application.Features.Componente.Dtos;

public class ComponenteDetail
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? CodigoImt { get; set; }
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public string? UrlDataSheet { get; set; }
    public bool EstadoEliminado { get; set; }
}
