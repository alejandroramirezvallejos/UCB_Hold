using System.Data;

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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear gavetero: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}, tipo={comando.Tipo}", ex);
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
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar gavetero: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_gavetero(
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
            throw new Exception($"Error en BD al eliminar gavetero: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_gaveteros()
        ";

        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener gaveteros: {innerError}. SQL: {sql}", ex);
        }
    }
}

