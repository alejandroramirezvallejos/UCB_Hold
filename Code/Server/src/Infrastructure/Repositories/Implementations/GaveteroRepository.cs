using System.Data;

public class GaveteroRepository : IGaveteroRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GaveteroRepository(IExecuteQuery ejecutarConsulta)
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
            throw new Exception("Error al crear el gavetero", ex);
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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el gavetero", ex);
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
            throw new Exception("Error al eliminar el gavetero", ex);
        }
    }

    public List<GaveteroDto> ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_gaveteros()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<GaveteroDto>(dt.Rows.Count);
        foreach (DataRow fila in dt.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }
    private GaveteroDto MapearFilaADto(DataRow fila)
    {
        return new GaveteroDto
        {
            Nombre = fila["nombre"] == DBNull.Value ? null : Convert.ToString(fila["nombre"]),
            Tipo = fila["tipo"] == DBNull.Value ? null : Convert.ToString(fila["tipo"]),
            NombreMueble = fila["nombre_mueble"] == DBNull.Value ? null : Convert.ToString(fila["nombre_mueble"]),
            Longitud = fila["longitud"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud"]),
            Profundidad = fila["profundidad"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad"]),
            Altura = fila["altura"] == DBNull.Value ? null : Convert.ToDouble(fila["altura"])
        };
    }
}

