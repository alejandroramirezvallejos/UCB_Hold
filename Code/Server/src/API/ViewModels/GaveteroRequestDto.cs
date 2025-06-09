namespace API.ViewModels;

public class GaveteroRequestDto
{
    public string?  Nombre        { get; set; } = null;
    public string? Tipo          { get; set; }= null;
    public string?  NombreMueble  { get; set; } = null;
    public double? Longitud      { get; set; }= null;
    public double? Profundidad   { get; set; }= null;
    public double? Altura        { get; set; }= null;
}