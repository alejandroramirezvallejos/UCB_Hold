namespace IMT_Reservas.Server.Application.Features.Comentario.Dtos;

public class ComentarioListDto
{
    public int Id { get; set; }
    public int IdGrupoEquipo { get; set; }
    public int IdUsuario { get; set; }
    public string? Contenido { get; set; }
    public int CantidadLikes { get; set; }
    public DateTime FechaCreacion { get; set; }
}
