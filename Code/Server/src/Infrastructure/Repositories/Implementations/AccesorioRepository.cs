using System.Data;

public class AccesorioRepository : IAccesorioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public AccesorioRepository(IExecuteQuery ejecutarConsulta)
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
            throw new Exception("Error al crear el accesorio", ex);
        }
    }

    public List<AccesorioDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_accesorios()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<AccesorioDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
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

    private static AccesorioDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Nombre = fila["nombre_accesorio"]==DBNull.Value? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
        };
    }
}