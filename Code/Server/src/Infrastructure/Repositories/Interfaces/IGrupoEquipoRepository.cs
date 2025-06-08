public interface IGrupoEquipoRepository
{
    void Crear(CrearGrupoEquipoComando comando);
    GrupoEquipoDto? ObtenerPorId(int id);
    List<GrupoEquipoDto> ObtenerPorNombreYCategoria(string? nombre, string? categoria);
    void Actualizar(ActualizarGrupoEquipoComando comando);
    void Eliminar(int id);
    List<GrupoEquipoDto> ObtenerTodos();
}