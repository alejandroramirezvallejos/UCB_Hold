using System.ComponentModel.DataAnnotations;

public class AccesorioRequestDto
{
    [Required(ErrorMessage = "El nombre del accesorio es requerido")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo del accesorio es requerido")]
    public string Modelo { get; set; } = string.Empty;

    public string? Tipo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El código IMT debe ser un número natural")]
    [Required(ErrorMessage = "El código IMT del accesorio es requerido")]
    public int CodigoIMT { get; set; }

    public string? Descripcion { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un número positivo")]
    public double? Precio { get; set; }

    [Url(ErrorMessage = "La URL del datasheet no tiene un formato válido")]
    public string? UrlDataSheet { get; set; }
}