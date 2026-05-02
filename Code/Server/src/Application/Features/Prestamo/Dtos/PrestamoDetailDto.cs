namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

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
