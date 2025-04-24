public enum EstadoPrestamo { Pendiente, Activo, Devuelto, Cancelado }

public record PrestamoCreateDto
{
    public DateTimeOffset FechaHoraSolicitud  { get; init; }
    public DateTimeOffset FechaHoraPrestamo   { get; init; }
    public DateTimeOffset FechaHoraDevolucion { get; init; }
    public string?        Observacion         { get; init; }
    public EstadoPrestamo Estado              { get; init; } = EstadoPrestamo.Pendiente;
}
