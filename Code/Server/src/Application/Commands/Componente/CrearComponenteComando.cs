namespace IMT_Reservas.Server.Application.Commands.Componente;

public record CrearComponenteComando(
    string? Nombre,
    string? Modelo,
    string? Tipo,
    int? CodigoIMT,
    string? Descripcion,
    double? PrecioReferencia,
    string? UrlDataSheet
);