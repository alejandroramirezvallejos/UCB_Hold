using System.ComponentModel.DataAnnotations;

public class CategoriaRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la categoria debe ser un numero natural")]
    public int     Id    { get; set; }
    
    [Required]
    public string Nombre { get; set; } = string.Empty;
}