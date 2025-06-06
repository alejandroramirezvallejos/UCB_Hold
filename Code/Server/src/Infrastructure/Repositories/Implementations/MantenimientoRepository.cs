//implementar
public class MantenimientoRepository : IMantenimientoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MantenimientoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }
    public MantenimientoDto Crear(CrearMantenimientoComando comando)
    {
        // Implementar lógica para crear un nuevo mantenimiento
    }

    public MantenimientoDto? ObtenerPorId(int id)
    {
        // Implementar lógica para obtener un mantenimiento por su ID
    }

    public MantenimientoDto? Actualizar(ActualizarMantenimientoComando comando)
    {
        // Implementar lógica para actualizar un mantenimiento existente
    }

    public bool Eliminar(int id)
    {
        // Implementar lógica para eliminar un mantenimiento por su ID
    }

    public List<MantenimientoDto> ObtenerTodos()
    {
        // Implementar lógica para obtener todos los mantenimientos
    }
}