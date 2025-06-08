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
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear categoría", ex);
        }
    }    public List<CategoriaDto>? Handle()
    {
        try
        {
            return _categoriaRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener categorías", ex);
        }
    }    public void Handle(ActualizarCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar categoría", ex);
        }
    }    public void Handle(EliminarCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar categoría", ex);
        }
    }
}