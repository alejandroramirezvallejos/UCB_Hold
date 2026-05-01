namespace IMT_Reservas.Server.Application.Commands.Accesorio;

public record CrearAccesorioComando(
    string? Nombre,
    string? Modelo,
    string? Tipo,
    int? CodigoIMT,
    string? Descripcion,
    double? Precio,
    string? UrlDataSheet
);
