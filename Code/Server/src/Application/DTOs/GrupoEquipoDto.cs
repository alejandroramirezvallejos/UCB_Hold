public class GrupoEquipoDto
{
    public int     Id              { get; set; }
    public string? Nombre          { get; set; } = null;
    public string? Modelo          { get; set; } = null;
    public string? Marca           { get; set; } = null;
    public string? NombreCategoria { get; set; } = null;
    public int?    Cantidad        { get; set; } = null;
    public string? Descripcion     { get; set; } = null;
    public string? UrlDataSheet    { get; set; } = null;
    public string? UrlImagen       { get; set; } = null;
}
