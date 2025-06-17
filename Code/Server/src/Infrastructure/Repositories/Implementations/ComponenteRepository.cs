using System.Data;
using Npgsql;
using Shared.Common;
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
        };          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "componente", parametros);
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
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "componente", parametros);
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
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "componente", parametros);
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
