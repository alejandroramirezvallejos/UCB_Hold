using System.Data;

public class PrestamoService : ICrearPrestamoComando, IObtenerPrestamoConsulta,
                               IActualizarPrestamoComando, IEliminarPrestamoComando
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public PrestamoDto Handle(CrearPrestamoComando comando)
    {
        return _prestamoRepository.Crear(comando);
    }

    public PrestamoDto? Handle(ObtenerPrestamoConsulta consulta)
    {
        return _prestamoRepository.ObtenerPorId(consulta.Id);
    }

    public PrestamoDto? Handle(ActualizarPrestamoComando comando)
    {
        return _prestamoRepository.Actualizar(comando);
    }

    public bool Handle(EliminarPrestamoComando comando)
    {
        return _prestamoRepository.Eliminar(comando.Id);
    }
}