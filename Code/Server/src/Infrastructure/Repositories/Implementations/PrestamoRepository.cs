using System.Data;
using Npgsql;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public PrestamoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    public int Crear(CrearPrestamoComando comando)
    {
        const string sql = @"CALL public.insertar_prestamo(
	    @grupoEquipoId,
	    @fechaPrestamoEsperada,
	    @fechaDevolucionEsperada,
	    @observacion,
	    @carnetUsuario,
	    @idContrato
        )";
        var parametros = new Dictionary<string, object?>
        {
            ["grupoEquipoId"] = comando.GrupoEquipoId ?? (object)DBNull.Value,
            ["fechaPrestamoEsperada"] = comando.FechaPrestamoEsperada.HasValue ? (object)comando.FechaPrestamoEsperada.Value : DBNull.Value,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada.HasValue ? (object)comando.FechaDevolucionEsperada.Value : DBNull.Value,
            ["observacion"] = comando.Observacion ?? (object)DBNull.Value,
            ["carnetUsuario"] = comando.CarnetUsuario ?? (object)DBNull.Value,
            ["idContrato"] =DBNull.Value
        };
        const string sqlId= @"Select id_prestamo from public.prestamos 
                        where fecha_prestamo_esperada = @fechaPrestamoEsperada
                        and fecha_devolucion_esperada = @fechaDevolucionEsperada
                        and carnet = @carnetUsuario
                        order by id_prestamo desc
                        limit 1";
        var parametrosId = new Dictionary<string, object?> {
            ["fechaPrestamoEsperada"] = comando.FechaPrestamoEsperada ?? (object)DBNull.Value,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada ?? (object)DBNull.Value,
            ["carnetUsuario"] = comando.CarnetUsuario ?? (object)DBNull.Value
        };
        
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            var dt= _ejecutarConsulta.EjecutarFuncion(sqlId, parametrosId);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value) return Convert.ToInt32(dt.Rows[0][0]);
            throw new Exception("Fallo crítico: No se pudo crear el préstamo y obtener el ID.");
        }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear préstamo: {ex.Message}", ex); }
    }
    public void Eliminar(int id)
    {
        const string sql = @"CALL public.eliminar_prestamo(@id)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar préstamo: {ex.Message}", ex); }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT * from public.obtener_prestamos()";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener préstamos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener préstamos: {ex.Message}", ex); }
    }
    public void AceptarPrestamo(int prestamoId)
    {
        const string sql = @"CALL public.aceptar_prestamo(@idPrestamo)";
        var parametros = new Dictionary<string, object?> { ["idPrestamo"] = prestamoId };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al aceptar el préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al aceptar el préstamo: {ex.Message}", ex); }
    }
    public DataTable ObtenerPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo)
    {
        const string sql = @"SELECT * from public.obtener_prestamos_por_carnet_y_estado_prestamo(@carnetUsuario,@estadoPrestamo::estado_prestamo)";
        var parametros = new Dictionary<string, object?>
        {
            ["carnetUsuario"] = carnetUsuario ?? (object)DBNull.Value,
            ["estadoPrestamo"] = estadoPrestamo ?? (object)DBNull.Value
        };
        try { return _ejecutarConsulta.EjecutarFuncion(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener préstamos por carnet y estado: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener préstamos por carnet y estado: {ex.Message}", ex); }
    }
    public void ActualizarEstado(ActualizarEstadoPrestamoComando comando)
    {
        const string sql = @"CALL public.actualizar_estado_prestamo(@idPrestamo,@estadoPrestamo::estado_prestamo)";
        var parametros = new Dictionary<string, object?>
        {
            ["idPrestamo"] = comando.Id,
            ["estadoPrestamo"] = comando.EstadoPrestamo
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar estado del préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar estado del préstamo: {ex.Message}", ex); }
    }
    public void ActualizarIdContrato(int prestamoId, string idContrato)
    {
        const string sql = @"UPDATE public.prestamos SET id_contrato = @idContrato WHERE id_prestamo = @idPrestamo";
        var parametros = new Dictionary<string, object?>
        {
            ["idPrestamo"] = prestamoId,
            ["idContrato"] = idContrato
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar el id del contrato: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar el id del contrato: {ex.Message}", ex); }
    }
}