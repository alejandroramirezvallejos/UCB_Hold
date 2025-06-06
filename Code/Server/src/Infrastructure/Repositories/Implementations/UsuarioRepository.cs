//implementar
public class UsuarioRepository : IUsuarioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public UsuarioRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public UsuarioDto Crear(CrearUsuarioComando comando)
    {
        // Implementar lógica para crear un nuevo usuario
    }

    public UsuarioDto? ObtenerPorId(int id)
    {
        // Implementar lógica para obtener un usuario por su ID
    }

    public UsuarioDto? Actualizar(ActualizarUsuarioComando comando)
    {
        // Implementar lógica para actualizar un usuario existente
    }

    public bool Eliminar(int id)
    {
        // Implementar lógica para eliminar un usuario por su ID
    }

    public List<UsuarioDto> ObtenerTodos()
    {
        // Implementar lógica para obtener todos los usuarios
    }
}