using System.Data;
using Npgsql;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;
using IMT_Reservas.Server.Application.ResponseDTOs;

public class PrestamoRepository :
    IEliminarRepository<EliminarPrestamoComando>,
    IObtenerTodosRepository<CrearPrestamoComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    private readonly MongoDbContexto _mongoDbContext;
    private readonly IGridFSBucket _gridFsBucket;
    
    public PrestamoRepository(IExecuteQuery ejecutarConsulta, MongoDbContexto mongoDbContext, IGridFSBucket gridFsBucket)
    {
        _ejecutarConsulta = ejecutarConsulta;
        _mongoDbContext = mongoDbContext;
        _gridFsBucket = gridFsBucket;
    }

    public int CrearPrestamo(CrearPrestamoComando comando)
    {
        const string sql = @"INSERT INTO public.prestamos (fecha_prestamo_esperada, fecha_devolucion_esperada, observacion, carnet, estado_eliminado)
                             VALUES (@fechaPrestamoEsperada, @fechaDevolucionEsperada, @observacion, @carnetUsuario, FALSE)
                             RETURNING id_prestamo";
        var parametros = new Dictionary<string, object?>
        {
            ["fechaPrestamoEsperada"] = comando.FechaPrestamoEsperada!.Value,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada!.Value,
            ["observacion"] = comando.Observacion ?? (object)DBNull.Value,
            ["carnetUsuario"] = comando.CarnetUsuario!
        };
        try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear préstamo: {ex.Message}", ex); }
    }

    public void CrearDetallePrestamo(int idPrestamo, int idEquipo)
    {
        const string sql = @"INSERT INTO public.detalles_prestamos (id_prestamo, id_equipo, estado_eliminado)
                             VALUES (@idPrestamo, @idEquipo, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["idPrestamo"] = idPrestamo,
            ["idEquipo"] = idEquipo
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear detalle préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear detalle préstamo: {ex.Message}", ex); }
    }

    public int? ObtenerEquipoDisponiblePorGrupo(int idGrupoEquipo, DateTime fechaPrestamoEsperada, DateTime fechaDevolucionEsperada)
    {
        // Buscar un equipo operativo del grupo que NO tenga préstamos activos/pendientes/aprobados que se solapen con las fechas solicitadas
        const string sql = @"SELECT e.id_equipo 
            FROM public.equipos AS e
            WHERE e.id_grupo_equipo = @idGrupoEquipo 
            AND e.estado_eliminado = FALSE 
            AND e.estado_equipo = 'operativo'
            AND e.id_equipo NOT IN (
                SELECT dp.id_equipo 
                FROM public.detalles_prestamos AS dp
                INNER JOIN public.prestamos AS p ON dp.id_prestamo = p.id_prestamo
                WHERE p.estado_eliminado = FALSE 
                AND dp.estado_eliminado = FALSE
                AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                AND p.fecha_prestamo_esperada < @fechaDevolucionEsperada
                AND p.fecha_devolucion_esperada > @fechaPrestamoEsperada
            )
            LIMIT 1";
        var parametros = new Dictionary<string, object?>
        {
            ["idGrupoEquipo"] = idGrupoEquipo,
            ["fechaPrestamoEsperada"] = fechaPrestamoEsperada,
            ["fechaDevolucionEsperada"] = fechaDevolucionEsperada
        };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public DataTable ObtenerEquipoPorId(int idEquipo)
    {
        const string sql = @"SELECT e.id_equipo, e.codigo_imt, e.numero_serial AS codigo_serial, 
            ge.nombre, ge.modelo, ge.marca, e.id_grupo_equipo
            FROM public.equipos AS e
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            WHERE e.id_equipo = @idEquipo AND e.estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?> { ["idEquipo"] = idEquipo };
        return _ejecutarConsulta.EjecutarFuncion(sql, parametros);
    }

    public void GuardarContrato(int idPrestamo, IFormFile contrato)
    {
        var fileName = contrato.FileName;
        using var stream = contrato.OpenReadStream();
        var fileId = _gridFsBucket.UploadFromStreamAsync(fileName, stream, null, default).GetAwaiter().GetResult();
        var contratoDoc = new Contrato { PrestamoId = idPrestamo, FileId = fileId.ToString() };
        _mongoDbContext.Contratos.InsertOneAsync(contratoDoc, null, default).GetAwaiter().GetResult();
        ActualizarIdContrato(idPrestamo, contratoDoc.FileId);
    }

    public void Eliminar(EliminarPrestamoComando comando)
    {
        // Eliminar detalles primero
        const string sqlDetalles = @"UPDATE public.detalles_prestamos SET estado_eliminado = TRUE WHERE id_prestamo = @id";
        var parametrosDetalles = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sqlDetalles, parametrosDetalles); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar detalles préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar detalles préstamo: {ex.Message}", ex); }

        // Eliminar préstamo
        const string sql = @"UPDATE public.prestamos SET estado_eliminado = TRUE WHERE id_prestamo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar préstamo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar préstamo: {ex.Message}", ex); }
    }
    
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT p.id_prestamo, u.carnet, u.nombre, u.apellido_paterno, u.telefono,
            ge.nombre AS nombre_grupo_equipo, CAST(e.codigo_imt AS TEXT) AS codigo_imt,
            p.fecha_solicitud, p.fecha_prestamo_esperada, p.fecha_prestamo,
            p.fecha_devolucion_esperada, p.fecha_devolucion, p.observacion,
            p.estado_prestamo, e.ubicacion AS ubicacion_equipo,
            g.nombre AS nombre_gavetero, m.nombre AS nombre_mueble, m.ubicacion AS ubicacion_mueble
            FROM public.prestamos AS p
            INNER JOIN public.usuarios AS u ON p.carnet = u.carnet
            INNER JOIN public.detalles_prestamos AS dp ON p.id_prestamo = dp.id_prestamo
            INNER JOIN public.equipos AS e ON dp.id_equipo = e.id_equipo
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            LEFT JOIN public.gaveteros AS g ON e.id_gavetero = g.id_gavetero
            LEFT JOIN public.muebles AS m ON g.id_mueble = m.id_mueble
            WHERE p.estado_eliminado = FALSE AND dp.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener préstamos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener préstamos: {ex.Message}", ex); }
    }
    
    public DataTable ObtenerPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo)
    {
        const string sql = @"SELECT p.id_prestamo, u.carnet, u.nombre, u.apellido_paterno, u.telefono,
            ge.nombre AS nombre_grupo_equipo, CAST(e.codigo_imt AS TEXT) AS codigo_imt,
            p.fecha_solicitud, p.fecha_prestamo_esperada, p.fecha_prestamo,
            p.fecha_devolucion_esperada, p.fecha_devolucion, p.observacion,
            p.estado_prestamo, e.ubicacion AS ubicacion_equipo,
            g.nombre AS nombre_gavetero, m.nombre AS nombre_mueble, m.ubicacion AS ubicacion_mueble
            FROM public.prestamos AS p
            INNER JOIN public.usuarios AS u ON p.carnet = u.carnet
            INNER JOIN public.detalles_prestamos AS dp ON p.id_prestamo = dp.id_prestamo
            INNER JOIN public.equipos AS e ON dp.id_equipo = e.id_equipo
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            LEFT JOIN public.gaveteros AS g ON e.id_gavetero = g.id_gavetero
            LEFT JOIN public.muebles AS m ON g.id_mueble = m.id_mueble
            WHERE p.estado_eliminado = FALSE AND dp.estado_eliminado = FALSE
            AND p.carnet = @carnetUsuario AND p.estado_prestamo = @estadoPrestamo::estado_prestamo";
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
        const string sql = @"UPDATE public.prestamos SET estado_prestamo = @estadoPrestamo::estado_prestamo WHERE id_prestamo = @idPrestamo AND estado_eliminado = FALSE";
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

    public List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta)
    {
        var contrato = _mongoDbContext.Contratos.Find(x => x.PrestamoId == consulta.PrestamoId && !x.EstadoEliminado).FirstOrDefault();
        if (contrato == null) throw new Exception($"No se encontró contrato para el préstamo {consulta.PrestamoId}");
        var fileObjectId = MongoDB.Bson.ObjectId.Parse(contrato.FileId);
        var chunksCollection = _mongoDbContext.BaseDeDatos.GetCollection<MongoDB.Bson.BsonDocument>("fs.chunks");
        var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("files_id", fileObjectId);
        var chunks = chunksCollection.Find(filter).SortBy(c => c["n"]).ToList();
        var dataChunks = new List<byte[]>();
        foreach (var chunk in chunks)
        {
            if (chunk.Contains("data"))
                dataChunks.Add(chunk["data"].AsBsonBinaryData.Bytes);
        }
        return dataChunks;
    }

    // --- Métodos auxiliares ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.prestamos WHERE id_prestamo = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteGrupoEquipoActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteUsuarioActivoPorCarnet(string carnet)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.usuarios WHERE carnet = @carnet AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["carnet"] = carnet };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }
}
