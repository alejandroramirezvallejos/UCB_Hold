namespace IMT_Reservas.Server.Application.Commands.Mueble;

public record ActualizarMuebleComando(
    int Id,
    string? Nombre,
    string? Tipo,
    double? Costo,
    string? Ubicacion,
    double? Longitud,
    double? Profundidad,
    double? Altura
);