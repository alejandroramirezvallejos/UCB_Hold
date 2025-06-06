public class GrupoEquipoDto
{
    public int    Id            { get; set; }
    public string Nombre        { get; set; } = string.Empty;
    public string Modelo        { get; set; } = string.Empty;
    public string? UrlData      { get; set; }
    public string UrlImagen     { get; set; } = string.Empty;
    public int    Cantidad      { get; set; }
    public string Marca         { get; set; } = string.Empty;
    public int    CategoriaId   { get; set; }
    public bool   EstaEliminado { get; set; }
    public string Descripcion   { get; set; } = string.Empty;
}
