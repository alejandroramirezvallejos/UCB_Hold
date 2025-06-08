public class MantenimientoService : IObtenerMantenimientoConsulta, ICrearMantenimientoComando,
                            IEliminarMantenimientoComando
{
    private readonly IMantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(IMantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }    public void Handle(CrearMantenimientoComando comando)
    {
        try
        {
            _mantenimientoRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear mantenimiento", ex);
        }
    }    public void Handle(EliminarMantenimientoComando comando)
    {
        try
        {
            _mantenimientoRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar mantenimiento", ex);
        }
    }    public List<MantenimientoDto>? Handle()
    {
        try
        {
            return _mantenimientoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener mantenimientos", ex);
        }
    }
}