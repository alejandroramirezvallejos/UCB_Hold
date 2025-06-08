using System.ComponentModel.DataAnnotations;
namespace API.ViewModels;

public class PrestamoRequestDto
{
    [Required(ErrorMessage = "Se debe especificar al menos un grupo de equipo")]
    [MinLength(1, ErrorMessage = "Se debe especificar al menos un grupo de equipo")]
    public int[]    GrupoEquipoId           { get; set; } = [];

    [Required(ErrorMessage = "La fecha de préstamo esperada es obligatoria")]
    [DataType(DataType.DateTime)]
    public DateTime FechaPrestamoEsperada   { get; set; }

    [Required(ErrorMessage = "La fecha de devolución esperada es obligatoria")]
    [DataType(DataType.DateTime)]
    public DateTime FechaDevolucionEsperada { get; set; }

    [StringLength(500, ErrorMessage = "La observación no puede exceder los 500 caracteres")]
    public string?  Observacion             { get; set; }

    [Required(ErrorMessage = "El carnet de usuario es obligatorio")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El carnet debe tener entre 3 y 20 caracteres")]
    public string   CarnetUsuario           { get; set; } = string.Empty;

    [MaxLength(10485760, ErrorMessage = "El contrato no puede exceder los 10MB")] // 10MB en bytes
    public byte[]?  Contrato                { get; set; }
}