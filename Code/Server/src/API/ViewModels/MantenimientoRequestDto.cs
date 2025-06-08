using System.ComponentModel.DataAnnotations;

public class MantenimientoRequestDto
{
    [Required(ErrorMessage = "La fecha de mantenimiento es requerida")]
    public DateOnly FechaMantenimiento { get; set; }

    [Required(ErrorMessage = "La fecha final de mantenimiento es requerida")]
    public DateOnly FechaFinalDeMantenimiento { get; set; }

    [Required(ErrorMessage = "El nombre de la empresa de mantenimiento es requerido")]
    [StringLength(100, ErrorMessage = "El nombre de la empresa no puede exceder 100 caracteres")]
    public string NombreEmpresaMantenimiento { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un número positivo")]
    public double? Costo { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? DescripcionMantenimiento { get; set; }

    [Required(ErrorMessage = "Al menos un código IMT es requerido")]
    [MinLength(1, ErrorMessage = "Debe proporcionar al menos un código IMT")]
    public int[] CodigoIMT { get; set; } = Array.Empty<int>();

    [Required(ErrorMessage = "Al menos un tipo de mantenimiento es requerido")]
    [MinLength(1, ErrorMessage = "Debe proporcionar al menos un tipo de mantenimiento")]
    public TipoDeMantenimiento[] TipoMantenimiento { get; set; } = Array.Empty<TipoDeMantenimiento>();

    public string[]? DescripcionEquipo { get; set; }
}