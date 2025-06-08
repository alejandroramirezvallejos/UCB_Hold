public record ActualizarAccesorioComando //No cambiar los use cases, estan justo como la bd
(
    int     Id,
    string?  Nombre,
    string? Modelo,
    string? Tipo,
    int?     CodigoIMT,
    string? Descripcion,
    double? Precio,
    string? UrlDataSheet
);
