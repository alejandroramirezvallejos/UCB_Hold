namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class PrestamoDetail
{
    public int Id { get; set; }
    public string? CarnetUsuario { get; set; }
    public string? EstadoPrestamo { get; set; }
    public DateTime? FechaSolicitud { get; set; }
    public DateTime? FechaDevolucionEsperada { get; set; }
    public string? NombreUsuario { get; set; }
    public string? ApellidoPaternoUsuario { get; set; }
    public string? TelefonoUsuario { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public int? CodigoImtEquipo { get; set; }
    public DateTime? FechaPrestamoEsperada { get; set; }
    public DateTime? FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? Observacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
