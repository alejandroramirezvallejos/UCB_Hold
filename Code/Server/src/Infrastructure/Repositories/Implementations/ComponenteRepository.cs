using System.Data;
using Npgsql;
public class ComponenteRepository : IComponenteRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public ComponenteRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearComponenteComando comando)
    {
        const string sql = @"
                CALL public.insertar_componente(
	            @nombre,
	            @modelo,
	            @tipo,
	            @codigoImt,
	            @descripcion,
	            @precioReferencia,
	            @urlDataSheet
                )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"] = comando.CodigoIMT,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear componente: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al crear componente: {ex.Message}", "crear", "componente", ex);
        }
    }
    public void Actualizar(ActualizarComponenteComando comando)
    {
        const string sql = @"
            CALL public.actualizar_componente(
	        @id,
	        @nombre,
	        @modelo,
	        @tipo,
	        @codigoImt,
	        @descripcion,
	        @precioReferencia,
	        @urlDataSheet
            )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"] = comando.CodigoIMT ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al actualizar componente: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al actualizar componente: {ex.Message}", "actualizar", "componente", ex);
        }
    }

    public void Eliminar(int id)
    {        const string sql = @"
            CALL public.eliminar_componente(
	        @id
            )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al eliminar componente: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al eliminar componente: {ex.Message}", "eliminar", "componente", ex);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_componentes()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }
    
}
