namespace IMT_Reservas.Server.Core.Entities;

public class Carrera : Entity
{
    public string Nombre { get; set; } = string.Empty;
    public bool EstadoEliminado { get; set; }
}
