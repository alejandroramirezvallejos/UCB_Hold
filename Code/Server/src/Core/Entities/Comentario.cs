namespace IMT_Reservas.Server.Core.Entities;

public class Comentario : Entity
{
    public string? Contenido { get; set; }
    public int IdGrupoEquipo { get; set; }
    public int IdUsuario { get; set; }
    public int CantidadLikes { get; set; }
    public bool EstadoEliminado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
