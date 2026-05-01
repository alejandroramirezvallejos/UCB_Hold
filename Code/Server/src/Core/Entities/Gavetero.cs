namespace IMT_Reservas.Server.Core.Entities;

public class Gavetero : Entity
{
    public string? Nombre { get; set; }
    public int IdMueble { get; set; }
    public bool EstadoEliminado { get; set; }
}
