using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class GaveteroRequestDto
{
    [Required]
    public string  Nombre        { get; set; } = string.Empty;

    public string? Tipo          { get; set; }

    [Required]
    public string  NombreMueble  { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "La longitud debe ser un numero positivo")]
    public double? Longitud      { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La profundidad debe ser un numero positivo")]
    public double? Profundidad   { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La altura debe ser un numero positivo")]
    public double? Altura        { get; set; }
}