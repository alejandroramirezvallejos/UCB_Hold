using System.Data;

public class EquipoRepository : IEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public EquipoRepository(IExecuteQuery ejecutarConsulta)
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
        };        try
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
        };        try
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
            throw new Exception($"Error en BD al eliminar equipo: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public List<EquipoDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_equipos()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            var lista = new List<EquipoDto>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                lista.Add(MapearFilaADto(row));
            return lista;
        }        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener equipos: {innerError}. SQL: {sql}", ex);
        }
    }
    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
        {
            Id = Convert.ToInt32(fila["id_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            CodigoImt = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            CodigoUcb = fila["codigo_ucb_equipo"] == DBNull.Value ? null : fila["codigo_ucb_equipo"].ToString(),
            Descripcion = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString(),
            NumeroSerial = fila["numero_serial_equipo"] == DBNull.Value ? null : fila["numero_serial_equipo"].ToString(),
            Ubicacion = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
            Procedencia = fila["procedencia_equipo"] == DBNull.Value ? null : fila["procedencia_equipo"].ToString(),
            TiempoMaximoPrestamo = fila["tiempo_max_prestamo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_max_prestamo_equipo"]),
            NombreGavetero = fila["nombre_gavetero_equipo"] == DBNull.Value ? null : fila["nombre_gavetero_equipo"].ToString(),
            EstadoEquipo = fila["estado_equipo_equipo"] == DBNull.Value ? null : fila["estado_equipo_equipo"].ToString(),
            CostoReferencia = fila["costo_referencia_equipo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia_equipo"]),
        };
    }
}