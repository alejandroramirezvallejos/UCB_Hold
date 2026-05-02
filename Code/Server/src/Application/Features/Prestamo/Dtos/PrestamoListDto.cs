namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class PrestamoListDto
{
	public int Id { get; set; }
	public string? CarnetUsuario { get; set; }
	public string? EstadoPrestamo { get; set; }
	public DateTime? FechaSolicitud { get; set; }
	public int? IdGrupoEquipo { get; set; }
	public DateTime? FechaDevolucionEsperada { get; set; }
}
