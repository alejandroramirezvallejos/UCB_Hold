namespace IMT_Reservas.Server.Core.Entities;

public class EmpresaMantenimiento : Entity
{
    public string? Nombre { get; set; }
    public string? Contacto { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public bool EstadoEliminado { get; set; }
}
