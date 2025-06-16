using System.Data;

public class EmpresaMantenimientoRepository : IEmpresaMantenimientoRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public EmpresaMantenimientoRepository(ExecuteQuery ejecutarConsulta)
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
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear empresa de mantenimiento: {innerError}. SQL: {sql}. Parámetros: nombre={comando.NombreEmpresa}, nit={comando.Nit}", ex);
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
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar empresa de mantenimiento: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.NombreEmpresa}", ex);
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
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al eliminar empresa de mantenimiento: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_empresas_mantenimiento()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener empresas de mantenimiento: {innerError}. SQL: {sql}", ex);
        }
    }
}