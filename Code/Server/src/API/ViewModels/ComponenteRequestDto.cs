using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class ComponenteRequestDto
{
    public int    Id                { get; set; }
    
    [Required(ErrorMessage = "El nombre del componente es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre            { get; set; } = string.Empty;

    [StringLength(50)]
    [Required(ErrorMessage = "El modelo del componente es requerido")]
    public string Modelo            { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Tipo             { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El código IMT debe ser mayor a 0")]
    [Required(ErrorMessage = "El código IMT del componente es requerido")]
    public int     CodigoIMT        { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion      { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio de referencia debe ser mayor a 0")]
    public double? PrecioReferencia { get; set; }
    
    [Url(ErrorMessage = "La URL debe tener un formato válido")]
    public string? UrlDataSheet     { get; set; }
}