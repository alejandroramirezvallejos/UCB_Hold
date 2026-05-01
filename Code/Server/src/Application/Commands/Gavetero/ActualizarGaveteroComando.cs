namespace IMT_Reservas.Server.Application.Commands.Gavetero;

public record ActualizarGaveteroComando(
    int Id,
    string? Nombre,
    string? Tipo,
    string? NombreMueble,
    double? Longitud,
    double? Profundidad,
    double? Altura
);
