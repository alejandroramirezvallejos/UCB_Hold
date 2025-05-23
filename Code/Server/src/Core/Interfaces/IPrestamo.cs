public interface IPrestamo
{
    int      Id                      { get; }
    DateTime FechaSolicitud          { get; }
    DateTime FechaPrestamo           { get; }
    DateTime FechaDevolucion         { get; }
    DateTime FechaDevolucionEsperada { get; }
    string?  Observacion             { get; }
    string   EstadoPrestamo          { get; }
    string   CarnetUsuario           { get; }
    int      EquipoId                { get; }
}
