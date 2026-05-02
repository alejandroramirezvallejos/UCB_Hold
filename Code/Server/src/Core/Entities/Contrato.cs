namespace IMT_Reservas.Server.Core.Entities;

public class Contrato : Entity
{
    public int PrestamoId { get; set; }
    public string? FileId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
