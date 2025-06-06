//implementar
public class MuebleRepository : IMuebleRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MuebleRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public MuebleDto Crear(CrearMuebleComando comando)
    {
        // Implementar lógica para crear un nuevo mueble
    }

    public MuebleDto? ObtenerPorId(int id)
    {
        // Implementar lógica para obtener un mueble por su ID
    }

    public MuebleDto? Actualizar(ActualizarMuebleComando comando)
    {
        // Implementar lógica para actualizar un mueble existente
    }

    public bool Eliminar(int id)
    {
        // Implementar lógica para eliminar un mueble por su ID
    }

    public List<MuebleDto> ObtenerTodos()
    {
        // Implementar lógica para obtener todos los muebles
    }
}