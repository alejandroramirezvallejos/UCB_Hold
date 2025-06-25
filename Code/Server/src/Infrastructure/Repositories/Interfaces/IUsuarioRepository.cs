using System.Data;
public interface IUsuarioRepository
{
    void Crear(CrearUsuarioComando comando);
    void Actualizar(ActualizarUsuarioComando comando);
    void Eliminar(string carnet);
    DataTable ObtenerTodos();
    DataTable? ObtenerPorEmailYContrasena(string email, string contrasena);
    DataTable ObtenerPorCarnets(List<string> carnets);
}