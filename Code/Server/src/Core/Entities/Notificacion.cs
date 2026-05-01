namespace IMT_Reservas.Server.Core.Entities;

public class Notificacion : Entity
{
    public int IdUsuario { get; set; }
    public string? Titulo { get; set; }
    public string? Contenido { get; set; }
    public bool EsLeido { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
