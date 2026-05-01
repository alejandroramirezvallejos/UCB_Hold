namespace IMT_Reservas.Server.Core.Entities;

public class GrupoEquipo : Entity
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int IdCategoria { get; set; }
    public bool EstadoEliminado { get; set; }
}
