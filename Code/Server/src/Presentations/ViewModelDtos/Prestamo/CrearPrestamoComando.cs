using Microsoft.AspNetCore.Http;

public record CrearPrestamoComando
(
    int[]?    GrupoEquipoId, //Se valida si no es nulo
    DateTime? FechaPrestamoEsperada, //Se valida si no es nulo
    DateTime? FechaDevolucionEsperada, //Se valida si no es nulo
    string?  Observacion,
    string?   CarnetUsuario, //Se valida si no es nulo
    IFormFile?  Contrato
);