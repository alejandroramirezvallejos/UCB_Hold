using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;
public class ActualizarUsuarioRequestDto
{
    [Required(ErrorMessage = "El carnet es obligatorio")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El carnet debe tener entre 3 y 20 caracteres")]
    public string Carnet               { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string? Nombre              { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido paterno debe tener entre 2 y 100 caracteres")]
    public string? ApellidoPaterno     { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido materno debe tener entre 2 y 100 caracteres")]
    public string? ApellidoMaterno     { get; set; }

    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
    public string? Email               { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    public string? Contrasena          { get; set; }

    [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
    public string? Rol                 { get; set; }

    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre de la carrera debe tener entre 3 y 150 caracteres")]
    public string? NombreCarrera       { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [StringLength(20, MinimumLength = 7, ErrorMessage = "El teléfono debe tener entre 7 y 20 caracteres")]
    public string? Telefono            { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono de referencia no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono de referencia no puede exceder los 20 caracteres")]
    public string? TelefonoReferencia  { get; set; }

    [StringLength(150, ErrorMessage = "El nombre de referencia no puede exceder los 150 caracteres")]
    public string? NombreReferencia    { get; set; }

    [EmailAddress(ErrorMessage = "El formato del email de referencia no es válido")]
    [StringLength(150, ErrorMessage = "El email de referencia no puede exceder los 150 caracteres")]
    public string? EmailReferencia     { get; set; }
}