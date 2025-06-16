using System.Data;
public class CategoriaService
{
    private readonly CategoriaRepository _categoriaRepository;
    public CategoriaService(CategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }
    public void CrearCategoria(CrearCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<CategoriaDto>? ObtenerTodasCategorias()
    {
        try
        {
            DataTable resultado = _categoriaRepository.ObtenerTodos();
            var lista = new List<CategoriaDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarCategoria(ActualizarCategoriaComando comando)
    {
        try
        {
            _categoriaRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarCategoria(EliminarCategoriaComando comando)
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
    private static CategoriaDto MapearFilaADto(DataRow fila)
    {
        return new CategoriaDto
        {
            Id = Convert.ToInt32(fila["id_categoria"]),
            Nombre = fila["categoria"] == DBNull.Value ? null : fila["categoria"].ToString(),
        };
    }
}