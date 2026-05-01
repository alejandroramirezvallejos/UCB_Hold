namespace IMT_Reservas.Server.Application.Commands.Prestamo;

public record ActualizarEstadoPrestamoComando(
    int? Id,
    string? EstadoPrestamo
);
