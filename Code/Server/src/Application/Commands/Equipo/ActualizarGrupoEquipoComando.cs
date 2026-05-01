namespace IMT_Reservas.Server.Application.Commands.Equipo;

public record ActualizarGrupoEquipoComando(
    int Id,
    string? Nombre,
    string? Modelo,
    string? Marca,
    string? Descripcion,
    string? NombreCategoria,
    string? UrlDataSheet,
    string? UrlImagen
);