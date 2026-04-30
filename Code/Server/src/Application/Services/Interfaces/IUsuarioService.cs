using Ardalis.Result;

public interface IUsuarioService
{
    Result<UsuarioDto> Crear(CrearUsuarioComando comando);
    Result<List<UsuarioDto>> ObtenerTodos();
    Result<UsuarioDto> Actualizar(ActualizarUsuarioComando comando);
    Result<UsuarioDto> Eliminar(EliminarUsuarioComando comando);
    UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta);
}
