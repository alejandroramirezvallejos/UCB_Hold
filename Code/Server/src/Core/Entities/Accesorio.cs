namespace IMT_Reservas.Server.Core.Entities;

public class Accesorio : Entity
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }
    public string? UrlDataSheet { get; set; }
    public decimal? Precio { get; set; }
    public int IdEquipo { get; set; }
    public string? Tipo { get; set; }
    public bool EstadoEliminado { get; set; }
}
