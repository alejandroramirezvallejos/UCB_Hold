namespace API.ViewModels;

public class AccesorioRequestDto
{
    public string?  Nombre       { get; set; } = null;
    public string?  Modelo       { get; set; } = null;
    public string? Tipo         { get; set; } = null;
    public int?     CodigoIMT    { get; set; } = null;
    public string? Descripcion  { get; set; } = null;
    public double? Precio       { get; set; } = null;
    public string? UrlDataSheet { get; set; } = null;
}