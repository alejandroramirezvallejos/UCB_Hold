namespace IMT_Reservas.Server.Application.Commands.Gavetero;

public record CrearGaveteroComando(
    string? Nombre,
    string? Tipo,
    string? NombreMueble,
    double? Longitud,
    double? Profundidad,
    double? Altura
);