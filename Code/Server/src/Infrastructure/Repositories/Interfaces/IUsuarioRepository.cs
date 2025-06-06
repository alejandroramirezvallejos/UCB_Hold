//implementar
public interface IUsuarioRepository
{
    UsuarioDto Crear(CrearUsuarioComando comando);
    UsuarioDto? ObtenerPorId(int id);
    UsuarioDto? Actualizar(ActualizarUsuarioComando comando);
    bool Eliminar(int id);
    List<UsuarioDto> ObtenerTodos();
}