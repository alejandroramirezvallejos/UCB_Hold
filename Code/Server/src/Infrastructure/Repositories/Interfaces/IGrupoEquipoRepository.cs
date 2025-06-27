using System.Data;
public interface IGrupoEquipoRepository
{
    void Crear(CrearGrupoEquipoComando comando);
    DataTable? ObtenerPorId(int id);
    DataTable ObtenerPorNombreYCategoria(string? nombre, string? categoria);
    void Actualizar(ActualizarGrupoEquipoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
    
    DataTable ObtenerFavoritosPorCarnetUsuario(string carnetUsuario);
    void MarcarComoFavorito(MarcarComoFavoritoComando comando);
}