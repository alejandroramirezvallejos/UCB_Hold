using System.Data;
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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear componente: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}, modelo={comando.Modelo}, codigoIMT={comando.CodigoIMT}", ex);
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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar componente: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_componente(
	        @id
            )";        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al eliminar componente: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_componentes()
        ";

        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener componentes: {innerError}. SQL: {sql}", ex);
        }
    }
    
}
