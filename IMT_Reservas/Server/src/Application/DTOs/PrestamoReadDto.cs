public record PrestamoReadDto
{
    public int            CodPrestamo         { get; init; }
    public DateTimeOffset FechaHoraSolicitud  { get; init; }
    public DateTimeOffset FechaHoraPrestamo   { get; init; }
    public DateTimeOffset FechaHoraDevolucion { get; init; }
    public EstadoPrestamo EstadoPrestamo      { get; init; }
}
