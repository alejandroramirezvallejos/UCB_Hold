namespace API.ViewModels;

public class ComponenteRequestDto
{
    public int?    Id                { get; set; }
    public string? Nombre            { get; set; }
    public string? Modelo            { get; set; }
    public string? Tipo              { get; set; }
    public int?    CodigoIMT         { get; set; }
    public string? Descripcion       { get; set; }
    public double? PrecioReferencia  { get; set; }
    public string? UrlDataSheet      { get; set; }
}