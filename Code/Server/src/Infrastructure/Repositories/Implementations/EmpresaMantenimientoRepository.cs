using System.Data;
using Npgsql;
using Shared.Common;

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
            ["nit"] = comando.Nit ?? (object)DBNull.Value        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "empresa de mantenimiento", parametros);
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
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "empresa de mantenimiento", parametros);
        }
    }

    public void Eliminar(int id)
    {        const string sql = @"
        CALL public.eliminar_empresas_mantenimiento(
	    @id
        )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "empresa de mantenimiento", parametros);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_empresas_mantenimiento()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }
}