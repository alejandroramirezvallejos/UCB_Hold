public class AccesorioDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; } = null;
    public string? Modelo { get; set; } = null;
    public string? Tipo { get; set; } = null;
    public double? Precio { get; set; } = null;
    public string? NombreEquipoAsociado { get; set; } = null;
    public int? CodigoImtEquipoAsociado { get; set; } = null;
    public string? Descripcion { get; set; } = null;
    public string? UrlDataSheet { get; set; } = null;
}