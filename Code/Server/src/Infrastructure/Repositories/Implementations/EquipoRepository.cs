using System.Data;
using Npgsql;

public class EquipoRepository : IEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public EquipoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    public void Crear(CrearEquipoComando comando)
    {
        const string sql = @"CALL public.insertar_equipo(@nombre,@modelo,@marca,@codigoUcb,@descripcion,@numeroSerial,@ubicacion,@procedencia,@costoReferencia,@tiempoMaximoPrestamo,@nombreGavetero)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.NombreGrupoEquipo,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["codigoUcb"] = comando.CodigoUcb ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"] = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["procedencia"] = comando.Procedencia ?? (object)DBNull.Value,
            ["costoReferencia"] = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["nombreGavetero"] = comando.NombreGavetero ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear equipo: {ex.Message}", ex); }
    }
    public void Actualizar(ActualizarEquipoComando comando)
    {
        const string sql = @"CALL public.actualizar_equipo(@id,@nombre,@modelo,@marca,@codigoUcb,@descripcion,@numeroSerial,@ubicacion,@procedencia,@costoReferencia,@tiempoMaximoPrestamo,@nombreGavetero,@estadoEquipo)";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.NombreGrupoEquipo ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["codigoUcb"] = comando.CodigoUcb ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"] = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["procedencia"] = comando.Procedencia ?? (object)DBNull.Value,
            ["costoReferencia"] = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["nombreGavetero"] = comando.NombreGavetero ?? (object)DBNull.Value,
            ["estadoEquipo"] = comando.EstadoEquipo ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar equipo: {ex.Message}", ex); }
    }
    public void Eliminar(int id)
    {
        const string sql = @"CALL public.eliminar_equipo(@id)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar equipo: {ex.Message}", ex); }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT * from public.obtener_equipos()";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener equipos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener equipos: {ex.Message}", ex); }
    }
}