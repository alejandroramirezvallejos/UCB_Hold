public class GaveteroService : IObtenerGaveteroConsulta, ICrearGaveteroComando,
                             IActualizarGaveteroComando, IEliminarGaveteroComando
{
    private readonly IGaveteroRepository _gaveteroRepository;

    public GaveteroService(IGaveteroRepository gaveteroRepository)
    {
        _gaveteroRepository = gaveteroRepository;
    }    public void Handle(CrearGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }    public List<GaveteroDto>? Handle()
    {
        try
        {
            return _gaveteroRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }
}