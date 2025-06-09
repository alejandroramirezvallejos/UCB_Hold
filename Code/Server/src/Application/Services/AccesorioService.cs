public class AccesorioService : ICrearAccesorioComando, IObtenerAccesorioConsulta,
                                IActualizarAccesorioComando,
                                IEliminarAccesorioComando
{
    private readonly IAccesorioRepository _accesorioRepository;
    public AccesorioService(IAccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }    public void Handle(CrearAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public List<AccesorioDto>? Handle()
    {
        try
        {
            return _accesorioRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}