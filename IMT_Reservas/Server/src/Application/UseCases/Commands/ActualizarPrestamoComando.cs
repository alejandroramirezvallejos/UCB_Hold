public record ActualizarPrestamoComando
(
    int      Id,
    DateTime FechaSolicitud,
    DateTime FechaPrestamo,
    DateTime FechaDevolucion,
    DateTime FechaDevolucionEsperada,
    string?  Observacion,
    string   EstadoPrestamo,
    string   CarnetUsuario,
    int      EquipoId
);