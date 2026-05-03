namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class PrestamoListDto
{
    public int Id { get; set; }
    public string? CarnetUsuario { get; set; }
    public string? EstadoPrestamo { get; set; }
    public DateTime? FechaSolicitud { get; set; }
    public int? IdGrupoEquipo { get; set; }
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
    public string? UbicacionEquipo { get; set; }
    public string? NombreGavetero { get; set; }
    public string? NombreMueble { get; set; }
    public string? UbicacionMueble { get; set; }
    public int? IdContrato { get; set; }
    public int? FileId { get; set; }
}

public class PrestamoDetailDto
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string? EstadoPrestamo { get; set; }
    public string? Observaciones { get; set; }
    public bool EstadoEliminado { get; set; }
}
