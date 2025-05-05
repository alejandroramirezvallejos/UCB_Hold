using System.ComponentModel.DataAnnotations;

public class GrupoEquipoRequestDto
{
    [Required]
    public string  Nombre      { get; set; } = string.Empty;

    [Required]
    public string  Modelo      { get; set; } = string.Empty;

    [Url(ErrorMessage = "La URL de datos no tiene un formato valido")]
    public string? UrlData     { get; set; } = null;

    [Required]
    [Url(ErrorMessage = "La URL de la imagen no tiene un formato valido")]
    public string  UrlImagen   { get; set; } = string.Empty;
   
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int     Cantidad    { get; set; }

    [Required]
    public string  Marca       { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la categoria debe ser un numero natural")]
    public int     CategoriaId { get; set; }
}
