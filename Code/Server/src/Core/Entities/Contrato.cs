namespace IMT_Reservas.Server.Core.Entities;

public class Contrato : Entity
{
    public int IdPrestamo { get; set; }
    public string? UrlDocumento { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
