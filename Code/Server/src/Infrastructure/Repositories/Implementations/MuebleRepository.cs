using System.Data;

public class MuebleRepository : IMuebleRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MuebleRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearMuebleComando comando)
    {
        const string sql = @"
        CALL public.insertar_mueble(
	    @nombre,
	    @tipo,
	    @costo,
	    @ubicacion,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
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
            throw new Exception("Error al crear el mueble", ex);
        }
    }


    public void Actualizar(ActualizarMuebleComando comando)
    {
        const string sql = @"
        CALL public.actualizar_mueble(
	    @id,
	    @nombre,
	    @tipo,
	    @costo,
	    @ubicacion,
	    @longitud,
	    @profundidad,
	    @altura
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
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
            throw new Exception("Error al actualizar el mueble", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_mueble(
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
            throw new Exception("Error al eliminar el mueble", ex);
        }
    }

    public List<MuebleDto> ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_muebles()
        ";

        var resultado = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        List<MuebleDto> muebles = new List<MuebleDto>();
        foreach (DataRow fila in resultado.Rows)
        {
            muebles.Add(mapearDto(fila));
        }
        return muebles;
    }
    private MuebleDto mapearDto(DataRow fila)
    {
        return new MuebleDto
        {
            Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
            NumeroGaveteros = fila["numero_gaveteros"] == DBNull.Value ? null : Convert.ToInt32(fila["numero_gaveteros"]),
            Ubicacion = fila["ubicacion"] == DBNull.Value ? null : fila["ubicacion"].ToString(),
            Tipo = fila["tipo"] == DBNull.Value ? null : fila["tipo"].ToString(),
            Costo = fila["costo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo"]),
            Longitud = fila["longitud"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud"]),
            Profundidad = fila["profundidad"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad"]),
            Altura = fila["altura"] == DBNull.Value ? null : Convert.ToDouble(fila["altura"])
        };
    }
}