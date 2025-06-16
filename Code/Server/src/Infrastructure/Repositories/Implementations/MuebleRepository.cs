using System.Data;

public class MuebleRepository : IMuebleRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;
    public MuebleRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearMuebleComando comando)
    {
        const string sql = @"
        CALL public.insertar_mueble(
	    @nombre,
	    @tipo,
	    @costo,
	    @ubicacion,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear mueble: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}, tipo={comando.Tipo}", ex);
        }
    }


    public void Actualizar(ActualizarMuebleComando comando)
    {
        const string sql = @"
        CALL public.actualizar_mueble(
	    @id,
	    @nombre,
	    @tipo,
	    @costo,
	    @ubicacion,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar mueble: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_mueble(
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
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al eliminar mueble: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_muebles()
        ";

        try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener muebles: {innerError}. SQL: {sql}", ex);
        }
    }
    
}