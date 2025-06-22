using System.Data;
using Npgsql;

public class EmpresaMantenimientoRepository : IEmpresaMantenimientoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public EmpresaMantenimientoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public void Crear(CrearEmpresaMantenimientoComando comando)
    {
        const string sql = @"CALL public.insertar_empresa_mantenimiento(@nombre,@nombreResponsable,@apellidoResponsable,@telefono,@direccion,@nit)";
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
        const string sql = @"CALL public.actualizar_empresa_mantenimiento(@id,@nombre,@nombreResponsable,@apellidoResponsable,@telefono,@direccion,@nit)";
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

    public void Eliminar(int id)
    {
        const string sql = @"CALL public.eliminar_empresas_mantenimiento(@id)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar empresa de mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar empresa de mantenimiento: {ex.Message}", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT * from public.obtener_empresas_mantenimiento()";
        return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
    }
}