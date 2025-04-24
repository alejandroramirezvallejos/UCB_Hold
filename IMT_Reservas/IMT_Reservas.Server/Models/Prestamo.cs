public enum PrestamoEstado { Pendiente, Rechazado, Aprobado, Activo, Finalizado, Cancelado }

public class Prestamo
{
    public int            Id              { get; private set; }
    public DateTimeOffset FechaSolicitud  { get; private set; }
    public DateTimeOffset FechaPrestamo   { get; private set; }
    public DateTimeOffset FechaDevolucion { get; private set; }
    public string         Observacion     { get; private set; }
    public PrestamoEstado EstadoPrestamo  { get; private set; }
    public string         CarnetUsuario   { get; private set; }
    public Usuario        Usuario         { get; private set; }
    public int            EquipoId        { get; private set; }
    public Equipo         Equipo          { get; private set; }
    public bool           EstaEliminado   { get; private set; }

    public Prestamo(DateTimeOffset fechaSolicitud, DateTimeOffset fechaPrestamo, 
                    DateTimeOffset fechaDevolucion, string observacion, PrestamoEstado estado, 
                    string carnetUsuario, int equipoId)
    {
        FechaSolicitud    = fechaSolicitud;
        FechaPrestamo     = fechaPrestamo;
        FechaDevolucion   = fechaDevolucion;
        Observacion       = observacion;
        EstadoPrestamo    = estado;
        CarnetUsuario     = carnetUsuario;
        EquipoId          = equipoId;
        EstaEliminado     = false;
    }
}
