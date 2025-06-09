public interface IUsuarioRepository
{
    void Crear(CrearUsuarioComando comando);
    void Actualizar(ActualizarUsuarioComando comando);
    void Eliminar(string carnet);
    List<UsuarioDto> ObtenerTodos();
    UsuarioDto? ObtenerPorEmailYContrasena(string email, string contrasena);
}