namespace IMT_Reservas.Server.Core.Entities;

public class Componente : Entity
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }
    public decimal? Precio { get; set; }
    public int IdEquipo { get; set; }
    public bool EstadoEliminado { get; set; }
}
