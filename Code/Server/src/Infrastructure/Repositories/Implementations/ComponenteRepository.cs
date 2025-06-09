using System.Data;
public class ComponenteRepository : IComponenteRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public ComponenteRepository(IExecuteQuery ejecutarConsulta)
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
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
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
            throw new Exception("Error al crear el componente", ex);
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
            throw new Exception("Error al actualizar el componente", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_componente(
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
            throw new Exception("Error al eliminar el componente", ex);
        }
    }

    public List<ComponenteDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_componentes()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<ComponenteDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
    }
    private static ComponenteDto MapearFilaADto(DataRow fila)
    {
        return new ComponenteDto
        {
            Nombre = fila["nombre_componente"] == DBNull.Value ? null : fila["nombre_componente"].ToString(),
            Modelo = fila["modelo_componente"] == DBNull.Value ? null : fila["modelo_componente"].ToString(),
            Tipo = fila["tipo_componente"] == DBNull.Value ? null : fila["tipo_componente"].ToString(),
            Descripcion = fila["descripcion_componente"] == DBNull.Value ? null : fila["descripcion_componente"].ToString(),
            PrecioReferencia = fila["precio_referencia_componente"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_referencia_componente"]),
            NombreEquipo = fila["nombre_equipo"] == DBNull.Value ? null : fila["nombre_equipo"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"])
        };
    }
}
