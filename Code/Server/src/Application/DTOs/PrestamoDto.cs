public class PrestamoDto
{
    public int       Id                      { get; set; }
    public string?   CarnetUsuario           { get; set; } = null;
    public string?   NombreUsuario           { get; set; } = null;
    public string?   ApellidoPaternoUsuario  { get; set; } = null;
    public string?   TelefonoUsuario         { get; set; } = null;
    public string?   NombreGrupoEquipo        { get; set; } = null;
    public string?   CodigoImt                { get; set; } = null;
    public DateTime? FechaSolicitud          { get; set; } = null;
    public DateTime? FechaPrestamoEsperada   { get; set; } = null;
    public DateTime? FechaPrestamo           { get; set; } = null;
    public DateTime? FechaDevolucionEsperada { get; set; } = null;
    public DateTime? FechaDevolucion         { get; set; } = null;
    public string?   Observacion              { get; set; } = null;
    public string?   EstadoPrestamo          { get; set; } = null;
    public string?   IdContrato              { get; set; } = null;
    public string?   FileId                  { get; set; } = null; // Nuevo campo para el id del archivo en MongoDB
}