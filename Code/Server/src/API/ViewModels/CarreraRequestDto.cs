using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class CarreraRequestDto
{
    public int    Id     { get; set; }
    [Required(ErrorMessage = "El nombre de la carrera es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}