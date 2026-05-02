namespace IMT_Reservas.Server.Application.Features.Notificacion.Dtos;

public class NotificacionDetailDto
{
	public int Id { get; set; }
	public int IdUsuario { get; set; }
	public string? Titulo { get; set; }
	public string? Contenido { get; set; }
	public bool EsLeido { get; set; }
	public DateTime FechaCreacion { get; set; }
	public bool EstadoEliminado { get; set; }
}
