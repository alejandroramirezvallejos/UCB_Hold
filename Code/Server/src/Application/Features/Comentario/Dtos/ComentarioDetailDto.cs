namespace IMT_Reservas.Server.Application.Features.Comentario.Dtos;

public class ComentarioDetailDto
{
	public int Id { get; set; }
	public string? Contenido { get; set; }
	public int IdGrupoEquipo { get; set; }
	public int IdUsuario { get; set; }
	public int CantidadLikes { get; set; }
	public bool EstadoEliminado { get; set; }
	public DateTime FechaCreacion { get; set; }
}
