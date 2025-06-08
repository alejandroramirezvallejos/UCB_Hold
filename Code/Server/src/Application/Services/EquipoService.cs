public class EquipoService : IObtenerEquipoConsulta, ICrearEquipoComando,
                             IActualizarEquipoComando, IEliminarEquipoComando
{
    private readonly IEquipoRepository _equipoRepository;

    public EquipoService(IEquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }    public void Handle(CrearEquipoComando comando)
    {
        try
        {
            _equipoRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear equipo", ex);
        }
    }    public void Handle(ActualizarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar equipo", ex);
        }
    }    public void Handle(EliminarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar equipo", ex);
        }
    }    public List<EquipoDto>? Handle()
    {
        try
        {
            return _equipoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener equipos", ex);
        }
    }
}