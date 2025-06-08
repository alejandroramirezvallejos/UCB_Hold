using System.Data;

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
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear préstamo", ex);
        }
    }    public List<PrestamoDto>? Handle()
    {
        try
        {
            return _prestamoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener préstamos", ex);
        }
    }    public void Handle(EliminarPrestamoComando comando)
    {
        try
        {
            _prestamoRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar préstamo", ex);
        }
    }
}