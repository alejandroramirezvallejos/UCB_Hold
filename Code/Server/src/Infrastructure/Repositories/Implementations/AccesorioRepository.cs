using System.Data;

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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear accesorio: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}, codigoIMT={comando.CodigoIMT}", ex);
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
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el accesorio", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_accesorio(
	    @id
        )";
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el accesorio", ex);
        }
    }
}