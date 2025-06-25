using System.Data;
using Npgsql;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;

public class PrestamoRepository : IPrestamoRepository
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
            ["idContrato"] = DBNull.Value
        };
        const string sqlId = @"SELECT id_prestamo FROM public.prestamos 
                        WHERE fecha_prestamo_esperada = @fechaPrestamoEsperada
                        AND fecha_devolucion_esperada = @fechaDevolucionEsperada
                        AND carnet = @carnetUsuario
                        ORDER BY id_prestamo DESC
                        LIMIT 1";
        var parametrosId = new Dictionary<string, object?> {
            ["fechaPrestamoEsperada"] = comando.FechaPrestamoEsperada ?? (object)DBNull.Value,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada ?? (object)DBNull.Value,
            ["carnetUsuario"] = comando.CarnetUsuario ?? (object)DBNull.Value
        };
        
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            var dt = _ejecutarConsulta.EjecutarFuncion(sqlId, parametrosId);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["id_prestamo"] != DBNull.Value)
            {
                var prestamoId = Convert.ToInt32(dt.Rows[0]["id_prestamo"]);
                
                // Manejar contrato si existe
                if (comando.Contrato != null)
                {
                    var fileName = comando.Contrato.FileName;
                    using var stream = comando.Contrato.OpenReadStream();
                    var fileId = _gridFsBucket.UploadFromStreamAsync(fileName, stream, null, default).GetAwaiter().GetResult();
                    var contrato = new Contrato { PrestamoId = prestamoId, FileId = fileId.ToString() };
                    _mongoDbContext.Contratos.InsertOneAsync(contrato, null, default).GetAwaiter().GetResult();
                    ActualizarIdContrato(prestamoId, contrato.FileId);
                }
                
                return prestamoId;
            }
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
}