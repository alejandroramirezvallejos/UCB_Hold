public class MuebleDto
{
    public int     Id              { get; set; }
    public string  Nombre          { get; set; } = string.Empty;
    public string? Tipo            { get; set; } = null;
    public string? Ubicacion       { get; set; } = null;
    public double? NumeroGaveteros { get; set; } = null;
    public double? Costo           { get; set; } = null;
    public double? Alto            { get; set; } = null;
    public double? Ancho           { get; set; } = null;
    public double? Largo           { get; set; } = null;
    public bool    EstaEliminado   { get; set; }
}