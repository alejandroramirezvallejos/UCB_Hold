public record CrearPrestamoComando
(
    int[]     GrupoEquipoId,
    DateTime FechaPrestamoEsperada,
    DateTime FechaDevolucionEsperada,
    string?  Observacion,
    string   CarnetUsuario,
    byte[]   Contrato
);