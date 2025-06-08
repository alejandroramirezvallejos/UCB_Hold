using System.Collections.Generic;
public class CategoriaService : ICrearCategoriaComando, IObtenerCategoriaConsulta,
                                IActualizarCategoriaComando,
                                IEliminarCategoriaComando
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public CategoriaDto Handle(CrearCategoriaComando comando)
    {
        return _categoriaRepository.Crear(comando);
    }

    public List<CategoriaDto>? Handle()
    {
        return _categoriaRepository.ObtenerTodos();
    }

    public CategoriaDto? Handle(ActualizarCategoriaComando comando)
        {
        return _categoriaRepository.Actualizar(comando);
    }

    public bool Handle(EliminarCategoriaComando comando)
    {
        return _categoriaRepository.Eliminar(comando.Id);
    }
}