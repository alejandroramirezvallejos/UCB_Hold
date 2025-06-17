using System.Data;
using Npgsql;
using Shared.Common;

public class AccesorioRepository : IAccesorioRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public AccesorioRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearAccesorioComando comando)
    {
        const string sql = @"
        CALL public.insertar_accesorios(
	    @nombre,
	    @modelo,
	    @tipo,
	    @codigoImt,
	    @descripcion,
	    @precio,
	    @urlDataSheet
        )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"]      = comando.Nombre,
            ["modelo"]      = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"]        = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"]   = comando.CodigoIMT,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precio"]      = comando.Precio ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value        
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "accesorio", parametros);
        }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_accesorios()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }

    public void Actualizar(ActualizarAccesorioComando comando)
    {
        const string sql = @"
            CALL public.actualizar_accesorio(
	        @id,
	        @nombre,
	        @modelo,
	        @tipo,
	        @codigoImt,
	        @descripcion,
	        @precio,
	        @urlDataSheet
            )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]          = comando.Id,
            ["nombre"]      = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"]      = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"]        = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"]   = comando.CodigoIMT ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precio"]      = comando.Precio ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "accesorio", parametros);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_accesorio(
	    @id
        )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "accesorio", parametros);
        }
    }
}