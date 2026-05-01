namespace IMT_Reservas.Server.Core.Entities;

public class Usuario : Entity
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Contrasena { get; set; }
    public int? IdCarrera { get; set; }
    public string? Rol { get; set; }
    public bool EstadoEliminado { get; set; }
}
