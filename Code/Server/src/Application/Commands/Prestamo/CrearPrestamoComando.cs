namespace IMT_Reservas.Server.Application.Commands.Prestamo;

using Microsoft.AspNetCore.Http;

public record CrearPrestamoComando
(
    int[]? GrupoEquipoId,
    DateTime? FechaPrestamoEsperada,
    DateTime? FechaDevolucionEsperada,
    string? Observacion,
    string? CarnetUsuario,
    IFormFile? Contrato
);