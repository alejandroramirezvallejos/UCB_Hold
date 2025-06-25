public class AccesorioDto : BaseDto
{
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public double? Precio { get; set; }
    public string? NombreEquipoAsociado { get; set; }
    public int? CodigoImtEquipoAsociado { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlDataSheet { get; set; }
}