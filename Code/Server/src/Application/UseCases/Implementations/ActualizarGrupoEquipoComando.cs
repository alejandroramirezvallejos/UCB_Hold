public record ActualizarGrupoEquipoComando
(
    int     Id,
    string  Nombre,
    string  Modelo,
    string? UrlData,
    string  UrlImagen,
    int     Cantidad,
    string  Marca,
    int     CategoriaId
);