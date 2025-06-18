using System.Data;
using Npgsql;

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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear préstamo: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al crear préstamo: {ex.Message}", ex);
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al eliminar préstamo: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al eliminar préstamo: {ex.Message}", ex);
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al obtener préstamos: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al obtener préstamos: {ex.Message}", ex);
        }
    }
}