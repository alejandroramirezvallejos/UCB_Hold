using System.Data;

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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear préstamo: {innerError}. SQL: {sql}. Parámetros: carnetUsuario={comando.CarnetUsuario}, fechaPrestamoEsperada={comando.FechaPrestamoEsperada}", ex);
        }
    }
    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_prestamo(
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
            throw new Exception($"Error en BD al eliminar préstamo: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }

    public DataTable ObtenerTodos()
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
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener préstamos: {innerError}. SQL: {sql}", ex);
        }
    }
}