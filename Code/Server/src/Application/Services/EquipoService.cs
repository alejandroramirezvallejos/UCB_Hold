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
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }    public List<EquipoDto>? Handle()
    {
        try
        {
            return _equipoRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }
}