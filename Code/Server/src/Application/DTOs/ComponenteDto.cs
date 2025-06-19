public class ComponenteDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; } = null;
    public string? Modelo { get; set; } = null;
    public string? Tipo { get; set; } = null;
    public string? Descripcion { get; set; } = null;
    public double? PrecioReferencia { get; set; } = null;
    public string? NombreEquipo { get; set; } = null;
    public int? CodigoImtEquipo { get; set; } = null;
    public string? UrlDataSheet { get; set; } = null;
}