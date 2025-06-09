public class CategoriaService : ICrearCategoriaComando, IObtenerCategoriaConsulta,
                                IActualizarCategoriaComando,
                                IEliminarCategoriaComando
{
    private readonly ICategoriaRepository _categoriaRepository;
    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }    public void Handle(CrearCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public List<CategoriaDto>? Handle()
    {
        try
        {
            return _categoriaRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}