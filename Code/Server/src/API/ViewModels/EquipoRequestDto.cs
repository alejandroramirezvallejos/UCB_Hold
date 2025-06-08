using System.ComponentModel.DataAnnotations;

public class EquipoRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de grupo de equipo debe ser un numero natural")]
    public int     GrupoEquipoId        { get; set; }

    [Required]
    public string  CodigoImt            { get; set; } = string.Empty;

    public string? CodigoUcb            { get; set; }

    public string? Descripcion          { get; set; }

    [Required]
    [EnumDataType(typeof(EstadoDelEquipo), ErrorMessage = "Estado del equipo invalido")]
    public string  EstadoEquipo         { get; set; } = string.Empty;

    public string? NumeroSerial         { get; set; }

    public string? Ubicacion            { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo de referencia debe ser un numero positivo")]
    public double? CostoReferencia      { get; set; }    [Range(0, int.MaxValue, ErrorMessage = "El tiempo maximo de prestamo debe ser un numero positivo")]
    public int?    TiempoMaximoPrestamo { get; set; }

    public string? Procedencia          { get; set; }

    public string? NombreGavetero       { get; set; }
}