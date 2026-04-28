using System.Data;
using Npgsql;

public class EmpresaMantenimientoRepository :
    ICrearRepository<CrearEmpresaMantenimientoComando>,
    IActualizarRepository<ActualizarEmpresaMantenimientoComando>,
    IEliminarRepository<EliminarEmpresaMantenimientoComando>,
    IObtenerTodosRepository<CrearEmpresaMantenimientoComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public EmpresaMantenimientoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    
    public void Crear(CrearEmpresaMantenimientoComando comando)
    {
        const string sql = @"INSERT INTO public.empresas_mantenimiento (nombre, nombre_responsable, apellido_responsable, telefono, direccion, nit, estado_eliminado)
                             VALUES (@nombre, @nombreResponsable, @apellidoResponsable, @telefono, @direccion, @nit, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.NombreEmpresa,
            ["nombreResponsable"] = comando.NombreResponsable ?? (object)DBNull.Value,
            ["apellidoResponsable"] = comando.ApellidoResponsable ?? (object)DBNull.Value,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["direccion"] = comando.Direccion ?? (object)DBNull.Value,
            ["nit"] = comando.Nit ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear empresa de mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear empresa de mantenimiento: {ex.Message}", ex); }
    }
    
    public void Actualizar(ActualizarEmpresaMantenimientoComando comando)
    {
        const string sql = @"UPDATE public.empresas_mantenimiento SET
            nombre = COALESCE(@nombre, nombre),
            nombre_responsable = COALESCE(@nombreResponsable, nombre_responsable),
            apellido_responsable = COALESCE(@apellidoResponsable, apellido_responsable),
            telefono = COALESCE(@telefono, telefono),
            direccion = COALESCE(@direccion, direccion),
            nit = COALESCE(@nit, nit)
            WHERE id_empresa_mantenimiento = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.NombreEmpresa ?? (object)DBNull.Value,
            ["nombreResponsable"] = comando.NombreResponsable ?? (object)DBNull.Value,
            ["apellidoResponsable"] = comando.ApellidoResponsable ?? (object)DBNull.Value,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["direccion"] = comando.Direccion ?? (object)DBNull.Value,
            ["nit"] = comando.Nit ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar empresa de mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar empresa de mantenimiento: {ex.Message}", ex); }
    }
    
    public void Eliminar(EliminarEmpresaMantenimientoComando comando)
    {
        const string sql = @"UPDATE public.empresas_mantenimiento SET estado_eliminado = TRUE WHERE id_empresa_mantenimiento = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar empresa de mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar empresa de mantenimiento: {ex.Message}", ex); }
    }
    
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT em.id_empresa_mantenimiento, em.nombre AS nombre_empresa,
            em.nombre_responsable AS nombre_responsable_empresa, em.apellido_responsable AS apellido_responsable_empresa,
            em.telefono AS telefono_empresa, em.nit AS nit_empresa, em.direccion AS direccion_empresa
            FROM public.empresas_mantenimiento AS em WHERE em.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener empresas de mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener empresas de mantenimiento: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares para la lógica de negocio en el servicio ---

    public bool ExisteActivaPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.empresas_mantenimiento WHERE id_empresa_mantenimiento = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.empresas_mantenimiento WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.empresas_mantenimiento WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_empresa_mantenimiento <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ReactivarEliminadaPorNombre(string nombre)
    {
        const string sql = @"UPDATE public.empresas_mantenimiento SET estado_eliminado = FALSE WHERE nombre = @nombre AND estado_eliminado = TRUE";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        try 
        { 
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            var checkSql = @"SELECT EXISTS(SELECT 1 FROM public.empresas_mantenimiento WHERE nombre = @nombre AND estado_eliminado = FALSE)";
            var dt = _ejecutarConsulta.EjecutarFuncion(checkSql, parametros);
            return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
        }
        catch { return false; }
    }

    public void EliminarLogicamentePorId(int id)
    {
        const string sql = @"UPDATE public.empresas_mantenimiento SET estado_eliminado = TRUE WHERE id_empresa_mantenimiento = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }

    public int? ObtenerIdPorNombre(string nombre)
    {
        const string sql = @"SELECT id_empresa_mantenimiento FROM public.empresas_mantenimiento WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}