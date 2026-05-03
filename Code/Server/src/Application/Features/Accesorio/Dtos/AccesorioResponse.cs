namespace IMT_Reservas.Server.Application.Features.Accesorio.Dtos;


public class AccesorioListDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public string? Descripcion { get; set; }
    public string? CodigoImt { get; set; }
    public decimal? Precio { get; set; }
    public string? UrlDataSheet { get; set; }
    public string? NombreEquipoAsociado { get; set; }
}




public class AccesorioDetailDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }
    public string? UrlDataSheet { get; set; }
    public decimal? Precio { get; set; }
    public int IdEquipo { get; set; }
    public string? Tipo { get; set; }
    public bool EstadoEliminado { get; set; }
}

