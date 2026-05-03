namespace IMT_Reservas.Server.Application.Features.Equipo.Dtos;

public class EquipoList
{
    public int Id { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public string? CodigoImt { get; set; }
    public string? CodigoUcb { get; set; }
    public string? NumeroSerial { get; set; }
    public string? Ubicacion { get; set; }
    public string? EstadoEquipo { get; set; }
    public string? NombreGavetero { get; set; }
    public decimal? CostoReferencia { get; set; }
    public string? Descripcion { get; set; }
    public int? TiempoMaximoPrestamo { get; set; }
    public string? Procedencia { get; set; }
}
