using System.Data;

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
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear gavetero", ex);
        }
    }    public void Handle(ActualizarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar gavetero", ex);
        }
    }    public void Handle(EliminarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar gavetero", ex);
        }
    }    public List<GaveteroDto>? Handle()
    {
        try
        {
            return _gaveteroRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener gaveteros", ex);
        }
    }
}