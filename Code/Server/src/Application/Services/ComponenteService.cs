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
        catch
        {
            throw;
        }
    }    public List<ComponenteDto>? Handle()
    {
        try
        {
            return _componenteRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}