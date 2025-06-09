namespace API.ViewModels;

public class MuebleRequestDto
{
    public string?  Nombre        { get; set; } = null;
    public string? Tipo          { get; set; } = null;
    public double? Costo         { get; set; } = null;
    public string? Ubicacion     { get; set; } = null;
    public double? Longitud      { get; set; } = null;
    public double? Profundidad   { get; set; } = null;
    public double? Altura        { get; set; } = null;
}