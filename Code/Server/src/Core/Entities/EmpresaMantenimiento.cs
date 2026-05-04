namespace IMT_Reservas.Server.Core.Entities;

public class EmpresaMantenimiento : Entity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? NombreResponsable { get; set; }
    public string? ApellidoResponsable { get; set; }
    public bool EstadoEliminado { get; set; }
}
