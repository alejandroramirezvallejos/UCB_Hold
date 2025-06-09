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
        catch
        {
            throw;
        }
    }    public void Handle(EliminarMantenimientoComando comando)
    {
        try
        {
            _mantenimientoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }    public List<MantenimientoDto>? Handle()
    {
        try
        {
            return _mantenimientoRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }
}