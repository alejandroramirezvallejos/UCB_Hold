using System.Data;

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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear equipo: {innerError}. SQL: {sql}. Parámetros: nombre={comando.NombreGrupoEquipo}, modelo={comando.Modelo}, marca={comando.Marca}, codigoUcb={comando.CodigoUcb}", ex);
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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar equipo: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.NombreGrupoEquipo}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_equipo(
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
            throw new Exception($"Error en BD al eliminar equipo: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_equipos()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener equipos: {innerError}. SQL: {sql}", ex);
        }
    }
    
}