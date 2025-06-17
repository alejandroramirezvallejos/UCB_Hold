using System.Data;
using Npgsql;
using Shared.Common;

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
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "gavetero", parametros);
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
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "gavetero", parametros);
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
        }        catch (Exception ex)
        {
            var parametrosEliminar = new Dictionary<string, object?> { ["id"] = id };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "gavetero", parametrosEliminar);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_gaveteros()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }
}

