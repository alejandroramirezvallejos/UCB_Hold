namespace IMT_Reservas.Server.Application.Dtos;

public class EquipoDto
{
    public string? NombreGrupoEquipo { get; set; }
    public string? ModeloGrupoEquipo { get; set; }
    public string? MarcaGrupoEquipo { get; set; }
    public string? CodigoUcb { get; set; }
    public string? Descripcion { get; set; }
    public string? NumeroSerial { get; set; }
    public string? Ubicacion { get; set; }
    public string? Procedencia { get; set; }
    public decimal? CostoReferencia { get; set; }
    public int? TiempoMaximoPrestamo { get; set; }
    public string? NombreGavetero { get; set; }
    public string? EstadoEquipo { get; set; }
}
