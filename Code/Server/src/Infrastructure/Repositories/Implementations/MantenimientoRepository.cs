using System.Data;

public class MantenimientoRepository : IMantenimientoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MantenimientoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }
    public void Crear(CrearMantenimientoComando comando)
    {
        const string sql = @"
        CALL public.insertar_mantenimiento(
		@fechaMantenimiento,
		@fechaFinalMantenimiento,
		@nombreEmpresa,
		@costo,
		@descripcion,
		@codigosImt,
		@tiposMantenimiento,
		@descripcionesEquipo
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["fechaMantenimiento"] = comando.FechaMantenimiento,
            ["fechaFinalMantenimiento"] = comando.FechaFinalDeMantenimiento,
            ["nombreEmpresa"] = comando.NombreEmpresaMantenimiento,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["descripcion"] = comando.DescripcionMantenimiento ?? (object)DBNull.Value,
            ["codigosImt"] = comando.CodigoIMT ?? (object)DBNull.Value,
            ["tiposMantenimiento"] = comando.TipoMantenimiento ?? (object)DBNull.Value,
            ["descripcionesEquipo"] = comando.DescripcionEquipo ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el mantenimiento", ex);
        }
    }
    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_mantenimiento(
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
            throw new Exception("Error al eliminar el mantenimiento", ex);
        }
    }

    public List<MantenimientoDto> ObtenerTodos()
    {
        const string sql = @"
        SELECT * FROM public.obtener_mantenimientos()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<MantenimientoDto>(dt.Rows.Count);
        foreach (DataRow fila in dt.Rows)
            lista.Add(MapearFila(fila));
        return lista;
    }
    private MantenimientoDto MapearFila(DataRow fila)
    {
        return new MantenimientoDto
        {
            FechaMantenimiento = fila["fecha_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_mantenimiento"])),
            FechaFinalDeMantenimiento = fila["fecha_final_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_final_mantenimiento"])),
            NombreEmpresaMantenimiento = fila["nombre_empresa_mantenimiento"] == DBNull.Value ? null : fila["nombre_empresa_mantenimiento"].ToString(),
            Costo = fila["costo_mantenimiento"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mantenimiento"]),
            Descripcion = fila["descripcion_mantenimiento"] == DBNull.Value ? null : fila["descripcion_mantenimiento"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            TipoMantenimiento = fila["tipo_detalle_mantenimiento"] == DBNull.Value ? null : fila["tipo_detalle_mantenimiento"].ToString(),
            DescripcionEquipo = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString()
        };
    }
}