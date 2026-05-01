namespace IMT_Reservas.Server.Application.Commands.Categoria;

public record ObtenerGrupoEquipoPorNombreYCategoriaConsulta(
    string? Nombre,
    string? Categoria
);