using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class EmpresaMantenimientoRequestDto
{
    public int     Id                  { get; set; }
    
    [Required(ErrorMessage = "El nombre de la empresa es requerido")]
    [StringLength(100, ErrorMessage = "El nombre de la empresa no puede exceder 100 caracteres")]
    public string NombreEmpresa        { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El nombre del responsable no puede exceder 50 caracteres")]
    public string? NombreResponsable   { get; set; }

    [StringLength(50, ErrorMessage = "El apellido del responsable no puede exceder 50 caracteres")]
    public string? ApellidoResponsable { get; set; }

    [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 caracteres")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Telefono            { get; set; }

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit                 { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    public string? Direccion           { get; set; }
}