using System.Data;
using Npgsql;
using Shared.Common;

public class EquipoRepository : IEquipoRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public EquipoRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearEquipoComando comando)
    {
        const string sql = @"
            CALL public.insertar_equipo(
	        @nombre,
	        @modelo,
	        @marca,
	        @codigoUcb,
	        @descripcion,
	        @numeroSerial,
	        @ubicacion,
	        @procedencia,
	        @costoReferencia,
	        @tiempoMaximoPrestamo,
	        @nombreGavetero
            )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>{
            ["nombre"]                = comando.NombreGrupoEquipo,
            ["modelo"]                = comando.Modelo ?? (object)DBNull.Value,
            ["marca"]                 = comando.Marca ?? (object)DBNull.Value,
            ["codigoUcb"]             = comando.CodigoUcb ?? (object)DBNull.Value,
            ["descripcion"]           = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"]          = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"]             = comando.Ubicacion ?? (object)DBNull.Value,
            ["procedencia"]           = comando.Procedencia ?? (object)DBNull.Value,
            ["costoReferencia"]       = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"]  = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["nombreGavetero"]        = comando.NombreGavetero ?? (object)DBNull.Value
        };          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "equipo", parametros);
        }
    }

    public void Actualizar(ActualizarEquipoComando comando)
    {
        const string sql = @"
        CALL public.actualizar_equipo(
	    @id,
	    @nombre,
	    @codigoUcb,
	    @descripcion,
	    @numeroSerial,
	    @ubicacion,
	    @procedencia,
	    @costoReferencia,
	    @tiempoMaximoPrestamo,
	    @nombreGavetero,
	    @estadoEquipo
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]                    = comando.Id,
            ["nombre"]                = comando.NombreGrupoEquipo ?? (object)DBNull.Value,
            ["codigoUcb"]             = comando.CodigoUcb ?? (object)DBNull.Value,
            ["descripcion"]           = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"]          = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"]             = comando.Ubicacion ?? (object)DBNull.Value,
            ["procedencia"]           = comando.Procedencia ?? (object)DBNull.Value,
            ["costoReferencia"]       = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"]  = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["nombreGavetero"]        = comando.NombreGavetero ?? (object)DBNull.Value,
            ["estadoEquipo"]          = comando.EstadoEquipo ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "equipo", parametros);
        }
    }

    public void Eliminar(int id)
    {        const string sql = @"
        CALL public.eliminar_equipo(
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "equipo", parametrosEliminar);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_equipos()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }
    
}