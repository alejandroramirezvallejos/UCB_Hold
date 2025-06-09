public class PrestamoService : ICrearPrestamoComando, IObtenerPrestamoConsulta,
                                IEliminarPrestamoComando
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }    public void Handle(CrearPrestamoComando comando)
    {
        try
        {
            _prestamoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public List<PrestamoDto>? Handle()
    {
        try
        {
            return _prestamoRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarPrestamoComando comando)
    {
        try
        {
            _prestamoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}