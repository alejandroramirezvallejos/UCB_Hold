using System.ComponentModel.DataAnnotations;

public class MuebleRequestDto
{
    [Required]
    public string  Nombre        { get; set; } = string.Empty;

    public string? Tipo          { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un numero positivo")]
    public double? Costo         { get; set; }

    public string? Ubicacion     { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La longitud debe ser un numero positivo")]
    public double? Longitud      { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La profundidad debe ser un numero positivo")]
    public double? Profundidad   { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La altura debe ser un numero positivo")]
    public double? Altura        { get; set; }
}