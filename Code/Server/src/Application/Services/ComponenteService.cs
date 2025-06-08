public class ComponenteService : ICrearComponenteComando, IObtenerComponenteConsulta,
                                IActualizarComponenteComando,
                                IEliminarComponenteComando
{
    private readonly IComponenteRepository _componenteRepository;
    public ComponenteService(IComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }    public void Handle(CrearComponenteComando comando)
    {
        try
        {
            _componenteRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear componente", ex);
        }
    }    public List<ComponenteDto>? Handle()
    {
        try
        {
            return _componenteRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener componentes", ex);
        }
    }    public void Handle(ActualizarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar componente", ex);
        }
    }    public void Handle(EliminarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar componente", ex);
        }
    }
}