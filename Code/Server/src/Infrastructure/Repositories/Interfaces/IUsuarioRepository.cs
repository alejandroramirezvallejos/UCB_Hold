using System.Data;
using Ardalis.Result;

public interface IUsuarioRepository
{
    Result<UsuarioDto?> Crear(CrearUsuarioComando comando);
    Result<UsuarioDto?> Crear(int idCarrera, CrearUsuarioComando comando);
    Result<UsuarioDto?> Actualizar(ActualizarUsuarioComando comando);
    Result<UsuarioDto?> Actualizar(int? idCarrera, ActualizarUsuarioComando comando);
    Result<UsuarioDto?> Eliminar(EliminarUsuarioComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorCarnet(string carnet);
    int? ObtenerCarreraIdPorNombre(string nombreCarrera);
    DataTable? ObtenerPorEmailYContrasena(string email, string contrasena);
    DataTable ObtenerPorCarnets(List<string> carnets);
}
