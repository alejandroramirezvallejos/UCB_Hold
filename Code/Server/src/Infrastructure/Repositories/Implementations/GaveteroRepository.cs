//implementar
public class GaveteroRepository : IGaveteroRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GaveteroRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public GaveteroDto Crear(CrearGaveteroComando comando)
    {
        // Implementar lógica para crear un nuevo gavetero
    }

    public GaveteroDto? ObtenerPorId(int id)
    {
        // Implementar lógica para obtener un gavetero por su ID
    }

    public GaveteroDto? Actualizar(ActualizarGaveteroComando comando)
    {
        // Implementar lógica para actualizar un gavetero existente
    }

    public bool Eliminar(int id)
    {
        // Implementar lógica para eliminar un gavetero por su ID
    }

    public List<GaveteroDto> ObtenerTodos()
    {
        // Implementar lógica para obtener todos los gaveteros
    }
}