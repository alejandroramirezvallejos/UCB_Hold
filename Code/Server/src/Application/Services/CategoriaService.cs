using System.Data;
using Shared.Common;
public class CategoriaService : ICategoriaService
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
            ValidarEntradaCreacion(comando);
            _categoriaRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "categoría", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre de la categoría");

        if (comando.Nombre.Length > 50)
            throw new ErrorLongitudInvalida("nombre de la categoría", 50);
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
    }    public void ActualizarCategoria(ActualizarCategoriaComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _categoriaRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.Nombre
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "categoría", parametros);
        }
    }

    public void EliminarCategoria(EliminarCategoriaComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _categoriaRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "categoría", parametros);
        }
    }

    private void ValidarEntradaActualizacion(ActualizarCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID de la categoría");

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre de la categoría");

        if (comando.Nombre.Length > 50)
            throw new ErrorLongitudInvalida("nombre de la categoría", 50);
    }

    private void ValidarEntradaEliminacion(EliminarCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID de la categoría");
        
        try{
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