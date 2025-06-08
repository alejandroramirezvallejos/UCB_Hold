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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el equipo", ex);
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
            throw new Exception("Error al actualizar el equipo", ex);
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
            throw new Exception("Error al eliminar el equipo", ex);
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
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los equipos", ex);
        }
    }
    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
        {
            NombreGrupoEquipo = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
            CodigoImt = fila["codigo_imt"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt"]),
            CodigoUcb = fila["codigo_ucb"] == DBNull.Value ? null : fila["codigo_ucb"].ToString(),
            Descripcion = fila["descripcion"] == DBNull.Value ? null : fila["descripcion"].ToString(),
            NumeroSerial = fila["numero_serial"] == DBNull.Value ? null : fila["numero_serial"].ToString(),
            Ubicacion = fila["ubicacion"] == DBNull.Value ? null : fila["ubicacion"].ToString(),
            Procedencia = fila["procedencia"] == DBNull.Value ? null : fila["procedencia"].ToString(),
            TiempoMaximoPrestamo = fila["tiempo_maximo_prestamo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_maximo_prestamo"]),
            NombreGavetero = fila["nombre_gavetero"] == DBNull.Value ? null : fila["nombre_gavetero"].ToString(),
            EstadoEquipo = fila["estado_equipo"] == DBNull.Value ? null : fila["estado_equipo"].ToString(),
            CostoReferencia = fila["costo_referencia"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia"]),
        };
    }
}