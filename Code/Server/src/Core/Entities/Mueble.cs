namespace IMT_Reservas.Server.Core.Entities;

public class Mueble : Entity
{
    public string? Nombre { get; set; }
    public string? Ubicacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
