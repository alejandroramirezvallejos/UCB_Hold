using System.Data;
public class CategoriaService : ICategoriaService
{
    private readonly CategoriaRepository _categoriaRepository;
    public CategoriaService(CategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }    public void CrearCategoria(CrearCategoriaComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos de la categoría son requeridos");

            if (string.IsNullOrWhiteSpace(comando.Nombre))
                throw new ArgumentException("El nombre de la categoría es requerido", nameof(comando.Nombre));

            if (comando.Nombre.Length > 50)
                throw new ArgumentException("El nombre de la categoría no puede exceder 50 caracteres", nameof(comando.Nombre));

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