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
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }    public List<MuebleDto>? Handle()
    {
        try
        {
            return _muebleRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }
}