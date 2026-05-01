namespace IMT_Reservas.Server.Core.Entities;

public class Equipo : Entity
{
    public int IdGrupoEquipo { get; set; }
    public int CodigoImt { get; set; }
    public int? IdGavetero { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public string? CodigoUcb { get; set; }
    public string? NumeroSerial { get; set; }
    public string? EstadoEquipo { get; set; }
    public string? Ubicacion { get; set; }
    public double? CostoReferencia { get; set; }
    public string? Descripcion { get; set; }
    public int? TiempoMaximoPrestamo { get; set; }
    public string? Procedencia { get; set; }
    public bool EstadoEliminado { get; set; }
}
