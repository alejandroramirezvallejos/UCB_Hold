public class PrestamoDto
{
    public int      Id                      { get; set; }
    public DateTime FechaSolicitud          { get; set; }
    public DateTime FechaPrestamo           { get; set; }
    public DateTime FechaDevolucion         { get; set; }
    public DateTime FechaDevolucionEsperada { get; set; }
    public string?  Observacion             { get; set; } = null;
    public string   EstadoPrestamo          { get; set; } = string.Empty;
    public string   CarnetUsuario           { get; set; } = string.Empty;
    public int      EquipoId                { get; set; }
    public bool     EstaEliminado           { get; set; } = false;
}