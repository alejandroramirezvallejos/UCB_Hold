public class AccesorioService : ICrearAccesorioComando, IObtenerAccesorioConsulta,
                                IActualizarAccesorioComando,
                                IEliminarAccesorioComando
{
    private readonly IAccesorioRepository _accesorioRepository;
    public AccesorioService(IAccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }

    public void Handle(CrearAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear el accesorio", ex);
        }
    }

    public List<AccesorioDto>? Handle()
    {
        try
        {
            return _accesorioRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener los accesorios", ex);
        }
    }

    public void Handle(ActualizarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar el accesorio", ex);
        }
    }

    public void Handle(EliminarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar el accesorio", ex);
        }
    }
}