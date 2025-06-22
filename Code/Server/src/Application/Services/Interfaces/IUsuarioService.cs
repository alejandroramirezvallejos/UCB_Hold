public interface IUsuarioService
{
    void CrearUsuario(CrearUsuarioComando comando);
    List<UsuarioDto>? ObtenerTodosUsuarios();
    void ActualizarUsuario(ActualizarUsuarioComando comando);
    void EliminarUsuario(EliminarUsuarioComando comando);
    UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta);
}
