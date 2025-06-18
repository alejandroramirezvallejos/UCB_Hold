using System.Data;
using Npgsql;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public CategoriaRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearCategoriaComando comando)
    {
        const string sql = @"
            CALL public.insertar_categoria(
	        @nombre
            )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear categoría: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al crear categoría: {ex.Message}", "crear", "categoría", ex);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_categorias()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }

    public void Actualizar(ActualizarCategoriaComando comando)
    {
        const string sql = @"
        CALL public.actualizar_categoria(
		@id,
		@nombre
        )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]     = comando.Id,
            ["nombre"] = comando.Nombre == null ? null : comando.Nombre
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al actualizar categoría: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al actualizar categoría: {ex.Message}", "actualizar", "categoría", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_categoria(
	    @id
        )";        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al eliminar categoría: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al eliminar categoría: {ex.Message}", "eliminar", "categoría", ex);
        }
    }
}