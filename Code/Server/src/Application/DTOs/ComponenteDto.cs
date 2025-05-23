public class ComponenteDto
{
    public int     Id               { get; set; }
    public string  Nombre           { get; set; } = string.Empty;
    public string? Descripcion      { get; set; } = null;
    public string? Modelo           { get; set; } = null;
    public string? Url              { get; set; } = null;
    public string? Tipo             { get; set; } = null;
    public double? PrecioReferencia { get; set; } = null;
    public int     EquipoId         { get; set; }
    public bool    EstaEliminado    { get; set; } = false;
}