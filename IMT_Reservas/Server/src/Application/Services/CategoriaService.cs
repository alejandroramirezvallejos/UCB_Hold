using System.Collections.Generic;

public class CategoriaService : ICrearCategoriaComando, IObtenerCategoriaConsulta,
                                IObtenerCategoriasConsulta, IActualizarCategoriaComando,
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

    public CategoriaDto? Handle(ObtenerCategoriaConsulta consulta)
    {
        return _categoriaRepository.ObtenerPorId(consulta.Id);
    }

    public List<CategoriaDto> Handle(ObtenerCategoriasConsulta consulta)
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