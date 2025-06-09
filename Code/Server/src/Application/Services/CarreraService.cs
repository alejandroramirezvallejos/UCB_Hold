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
        catch
        {
            throw;
        }
    }    public List<CarreraDto>? Handle()
    {
        try
        {
            return _carreraRepository.ObtenerTodas();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}