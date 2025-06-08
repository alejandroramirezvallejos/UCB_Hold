using System.Data;

public class EmpresaMantenimientoRepository : IEmpresaMantenimientoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public EmpresaMantenimientoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearEmpresaMantenimientoComando comando)
    {
        const string sql = @"
        CALL public.insertar_empresa_mantenimiento(
	    @nombre,
	    @nombreResponsable,
	    @apellidoResponsable,
	    @telefono,
	    @direccion,
	    @nit
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.NombreEmpresa,
            ["nombreResponsable"] = comando.NombreResponsable ?? (object)DBNull.Value,
            ["apellidoResponsable"] = comando.ApellidoResponsable ?? (object)DBNull.Value,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["direccion"] = comando.Direccion ?? (object)DBNull.Value,
            ["nit"] = comando.Nit ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear la empresa de mantenimiento", ex);
        }
    }


    public void Actualizar(ActualizarEmpresaMantenimientoComando comando)
    {
        const string sql = @"
        CALL public.actualizar_empresa_mantenimiento(
	    @id,
	    @nombre,
	    @nombreResponsable,
	    @apellidoResponsable,
	    @telefono,
	    @direccion,
	    @nit
        )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.NombreEmpresa ?? (object)DBNull.Value,
            ["nombreResponsable"] = comando.NombreResponsable ?? (object)DBNull.Value,
            ["apellidoResponsable"] = comando.ApellidoResponsable ?? (object)DBNull.Value,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["direccion"] = comando.Direccion ?? (object)DBNull.Value,
            ["nit"] = comando.Nit ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar la empresa de mantenimiento", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_empresas_mantenimiento(
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
            throw new Exception("Error al eliminar la empresa de mantenimiento", ex);
        }
    }

    public List<EmpresaMantenimientoDto> ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_empresas_mantenimiento()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            var lista = new List<EmpresaMantenimientoDto>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                lista.Add(MapearFilaADto(row));
            return lista;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las empresas de mantenimiento", ex);
        }
    }

    private static EmpresaMantenimientoDto MapearFilaADto(DataRow fila)
    {
        return new EmpresaMantenimientoDto
        {
            NombreEmpresa = fila["nombre"].ToString() ?? string.Empty,
            NombreResponsable = fila["nombre_responsable"].ToString() ?? string.Empty,
            ApellidoResponsable = fila["apellido_responsable"].ToString() ?? string.Empty,
            Telefono = fila["telefono"].ToString() ?? string.Empty,
            Direccion = fila["direccion"].ToString() ?? string.Empty,
            Nit = fila["nit"].ToString() ?? string.Empty
        };
    }
}