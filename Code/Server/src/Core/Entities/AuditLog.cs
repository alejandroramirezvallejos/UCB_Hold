namespace IMT_Reservas.Server.Core.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string AdminCarnet { get; set; } = string.Empty;
    public string AdminNombre { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string? EntidadId { get; set; }
    public string? Detalle { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
