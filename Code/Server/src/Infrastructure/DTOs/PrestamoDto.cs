public class PrestamoDto
{
    public string?   CarnetUsuario           { get; set; } = null;
    public string?   NombreUsuario           { get; set; } = null;
    public string?   ApellidoPaternoUsuario  { get; set; } = null;
    public string?   TelefonoUsuario         { get; set; } = null;
    public string?  NombreGrupoEquipo      { get; set; } = null;
    public string?  CodigoImt               { get; set; } = null;
    public DateTime? FechaSolicitud { get; set; } = null;
    public DateTime? FechaPrestamoEsperada           { get; set; } = null;
    public DateTime? FechaPrestamo         { get; set; } = null;
    public DateTime? FechaDevolucionEsperada { get; set; } = null;
    public DateTime? FechaDevolucion { get; set; } = null;
    public string?  Observacion             { get; set; } = null;
    public string?   EstadoPrestamo          { get; set; } = null;
}