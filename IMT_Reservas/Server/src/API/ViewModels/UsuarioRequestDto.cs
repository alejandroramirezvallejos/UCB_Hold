using System.ComponentModel.DataAnnotations;
public class UsuarioRequestDto
{
    [Required]
    public string  Carnet             { get; set; } = string.Empty;

    [Required]
    public string  Nombre             { get; set; } = string.Empty;

    [Required]
    public string  ApellidoPaterno    { get; set; } = string.Empty;

    [Required]
    public string  ApellidoMaterno    { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(TipoDeUsuario), ErrorMessage = "Rol invalido")]
    public string  Rol                { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Carrera invalida")]
    public int     CarreraId          { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Contraseña invalida")]
    public string  Contrasena         { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Email invalido")]
    public string  Email              { get; set; } = string.Empty;

    [Required]
    [Phone(ErrorMessage = "Telefono invalido")]
    public string  Telefono           { get; set; } = string.Empty;

    public string? NombreReferencia   { get; set; } = null;
    public string? TelefonoReferencia { get; set; } = null;

    [EmailAddress(ErrorMessage = "Email de referencia invalido")]
    public string? EmailReferencia    { get; set; } = null;   
}