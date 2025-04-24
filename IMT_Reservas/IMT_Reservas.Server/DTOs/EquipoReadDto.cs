public class EquipoReadDto
{
    public int Clave { get; set; }
    public string CodigoEquipo { get; set; }
    public string CodigoIMT { get; set; }
    public string CodigoUCB { get; set; }
    public string Descripcion { get; set; }
    public string EstadoTecnico { get; set; }
    public string NumeroSerial { get; set; }
    public string Ubicacion { get; set; }
    public decimal? CostoReferencia { get; set; }
    public int? TiempoMaximoPrestamo { get; set; }
    public string Procedencia { get; set; }
    public bool Disponible { get; set; }
}