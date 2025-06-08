using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class GrupoEquipoRequestDto
{
    [Required(ErrorMessage = "El nombre del grupo de equipo es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre          { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo del grupo de equipo es requerido")]
    [StringLength(100, ErrorMessage = "El modelo no puede exceder 100 caracteres")]
    public string Modelo          { get; set; } = string.Empty;

    [Required(ErrorMessage = "La marca del grupo de equipo es requerida")]
    [StringLength(100, ErrorMessage = "La marca no puede exceder 100 caracteres")]
    public string Marca           { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [StringLength(100, ErrorMessage = "El nombre de la categoría no puede exceder 100 caracteres")]
    public string NombreCategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción del grupo de equipo es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Descripcion     { get; set; } = string.Empty;

    [Url(ErrorMessage = "La URL del datasheet no tiene un formato válido")]
    public string? UrlDataSheet   { get; set; }

    [Required(ErrorMessage = "La URL de la imagen es requerida")]
    [Url(ErrorMessage = "La URL de la imagen no tiene un formato válido")]
    public string UrlImagen       { get; set; } = string.Empty;
}

