using System.Data;
using Npgsql;
using Shared.Common;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;
    public PrestamoRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }
    public void Crear(CrearPrestamoComando comando)
    {
        const string sql = @"
            CALL public.insertar_prestamo(
            @GrupoEquipoId,
            @fechaPrestamoEsperada,
            @fechaDevolucionEsperada,
            @observacion,
            @carnetUsuario,
            @contrato)";

        var parametros = new Dictionary<string, object?>
        {
            ["GrupoEquipoId"] = comando.GrupoEquipoId,
            ["fechaPrestamoEsperada"] = comando.FechaPrestamoEsperada,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada,
            ["observacion"] = comando.Observacion ?? (object)DBNull.Value,
            ["carnetUsuario"] = comando.CarnetUsuario ?? (object)DBNull.Value,
            ["contrato"] = comando.Contrato ?? (object)DBNull.Value
        };          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "préstamo", parametros);
        }
    }
    public void Eliminar(int id)
    {        const string sql = @"
        CALL public.eliminar_prestamo(
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "préstamo", parametros);
        }
    }      public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_prestamos()
        ";

        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "préstamos", new Dictionary<string, object?>());
        }
    }
}