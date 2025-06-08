public class CarreraService : ICrearCarreraComando, IObtenerCarreraConsulta,
                                IActualizarCarreraComando,
                                IEliminarCarreraComando
{
    private readonly ICarreraRepository _carreraRepository;
    public CarreraService(ICarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }    public void Handle(CrearCarreraComando comando)
    {
        try
        {
            _carreraRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear carrera", ex);
        }
    }    public List<CarreraDto>? Handle()
    {
        try
        {
            return _carreraRepository.ObtenerTodas();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener carreras", ex);
        }
    }    public void Handle(ActualizarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar carrera", ex);
        }
    }    public void Handle(EliminarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar carrera", ex);
        }
    }
}