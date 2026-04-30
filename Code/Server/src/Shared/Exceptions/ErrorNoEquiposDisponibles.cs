
public class ErrorNoEquiposDisponibles : DomainException
{
    public ErrorNoEquiposDisponibles()
        : base("No hay equipos disponibles para préstamo en las fechas solicitadas")
    {
    }
}

