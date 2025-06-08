public class MuebleService : IObtenerMuebleConsulta, ICrearMuebleComando,
                             IActualizarMuebleComando, IEliminarMuebleComando
{
    private readonly IMuebleRepository _muebleRepository;

    public MuebleService(IMuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }    public void Handle(CrearMuebleComando comando)
    {
        try
        {
            _muebleRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear mueble", ex);
        }
    }    public void Handle(ActualizarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar mueble", ex);
        }
    }    public void Handle(EliminarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar mueble", ex);
        }
    }    public List<MuebleDto>? Handle()
    {
        try
        {
            return _muebleRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener muebles", ex);
        }
    }
}