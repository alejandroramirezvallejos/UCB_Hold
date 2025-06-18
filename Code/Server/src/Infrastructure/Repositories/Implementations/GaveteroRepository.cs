using System.Data;
using Npgsql;

public class GaveteroRepository : IGaveteroRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;
    public GaveteroRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearGaveteroComando comando)
    {
        const string sql = @"
        CALL public.insertar_gavetero(
	    @nombre,
	    @tipo,
	    @nombreMueble,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["nombreMueble"] = comando.NombreMueble ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear gavetero: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al crear gavetero: {ex.Message}", ex);
        }
    }


    public void Actualizar(ActualizarGaveteroComando comando)
    {
        const string sql = @"
        CALL public.actualizar_gavetero(
	    @id,
	    @nombre,
	    @tipo,
	    @nombreMueble,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["nombreMueble"] = comando.NombreMueble ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value        };
        
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al actualizar gavetero: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al actualizar gavetero: {ex.Message}", ex);
        }
    }

    public void Eliminar(int id)
    {        const string sql = @"
        CALL public.eliminar_gavetero(
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
            throw new ErrorDataBase($"Error de base de datos al eliminar gavetero: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al eliminar gavetero: {ex.Message}", ex);
        }
    }      public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_gaveteros()
        ";

        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al obtener gaveteros: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al obtener gaveteros: {ex.Message}", ex);
        }
    }
}

