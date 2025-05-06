using System.ComponentModel.DataAnnotations;

public class PrestamoRequestDto
{
    [Required]
    public DateTime FechaSolicitud          { get; set; }

    [Required]
    public DateTime FechaPrestamo           { get; set; }

    [Required]
    public DateTime FechaDevolucion         { get; set; }

    [Required]
    public DateTime FechaDevolucionEsperada { get; set; }

    public string?  Observacion             { get; set; } = null;

    [Required]
    [EnumDataType(typeof(EstadoDelPrestamo), ErrorMessage = "Estado de prestamo invalido")]
    public string   EstadoPrestamo          { get; set; } = string.Empty;

    [Required]
    public string   CarnetUsuario           { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int      EquipoId                { get; set; }
}