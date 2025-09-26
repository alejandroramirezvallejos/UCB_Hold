public class PrestamoDto : BaseDto
{
    public string? CarnetUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string? ApellidoPaternoUsuario { get; set; }
    public string? TelefonoUsuario { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public string? CodigoImt { get; set; }
    public DateTime? FechaSolicitud { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
    public DateTime? FechaPrestamo { get; set; }
    public DateTime? FechaDevolucionEsperada { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? Observacion { get; set; }
    public string? EstadoPrestamo { get; set; }
    public string? IdContrato { get; set; }
    public string? FileId { get; set; }

    public string? Ubicacion_Equipo { get; set; }
    public string? Nombre_Gavetero { get; set; }
    public string? Nombre_Mueble { get; set; }
    public string? Ubicacion_Mueble { get; set; }
}