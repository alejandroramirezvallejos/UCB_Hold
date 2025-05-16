public interface IGrupoEquipoRepository
{
    GrupoEquipoDto Crear(CrearGrupoEquipoComando comando);
    GrupoEquipoDto? ObtenerPorId(int id);
    List<Dictionary<string, object?>> ObtenerPorNombreYCategoria(string? nombre, string? categoria);
    GrupoEquipoDto? Actualizar(ActualizarGrupoEquipoComando comando);
    bool Eliminar(int id);
}