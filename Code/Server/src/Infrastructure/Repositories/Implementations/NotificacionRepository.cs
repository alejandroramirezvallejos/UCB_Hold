using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using IMT_Reservas.Server.Infrastructure.MongoDb;

public class NotificacionRepository : INotificacionRepository
{
    private readonly IMongoCollection<BsonDocument> _coleccion;
    public NotificacionRepository(MongoDbContexto contexto) => _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("notificaciones");

    public void Crear(CrearNotificacionComando comando)
    {
        var doc = new BsonDocument
        {
            ["CarnetUsuario"] = comando.CarnetUsuario,
            ["Titulo"] = comando.Titulo,
            ["Contenido"] = comando.Contenido,
            ["FechaEnvio"] = DateTime.UtcNow,
            ["Leido"] = false,
            ["EstadoEliminado"] = false
        };
        try { _coleccion.InsertOne(doc); }
        catch (Exception ex) { throw new ErrorRepository($"Error al crear la notificación: {ex.Message}", ex); }
    }

    public void Eliminar(EliminarNotificacionComando comando)
    {
        try {
            var filtro = new BsonDocument { ["_id"] = new ObjectId(comando.Id) };
            var actualizacion = Builders<BsonDocument>.Update.Set("EstadoEliminado", true);
            _coleccion.UpdateOne(filtro, actualizacion);
        }
        catch (FormatException ex) { throw new ErrorDataBase($"ID de notificación inválido: {ex.Message}", null, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error al eliminar la notificación: {ex.Message}", ex); }
    }

    public DataTable ObtenerPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta)
    {
        var filtro = new BsonDocument
        {
            ["CarnetUsuario"] = consulta.CarnetUsuario,
            ["EstadoEliminado"] = false
        };
        return ObtenerNotificaciones(filtro);
    }

    public void MarcarComoLeida(MarcarComoLeidoComando comando)
    {
        try {
            var filtro = new BsonDocument { ["_id"] = new ObjectId(comando.Id) };
            var actualizacion = Builders<BsonDocument>.Update.Set("Leido", true);
            _coleccion.UpdateOne(filtro, actualizacion);
        }
        catch (FormatException ex) { throw new ErrorDataBase($"ID de notificación inválido: {ex.Message}", null, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error al marcar la notificación como leída: {ex.Message}", ex); }
    }

    public bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta)
    {
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("CarnetUsuario", consulta.CarnetUsuario),
            Builders<BsonDocument>.Filter.Eq("Leido", false),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        try
        {
            // Usar FindSync en vez de Find para que sea mockeable y evitar problemas con argumentos opcionales/metodos de extensión
            using (var cursor = _coleccion.FindSync(filtro, new FindOptions<BsonDocument, BsonDocument> { Limit = 1 }))
            {
                return cursor.MoveNext() && cursor.Current.Any();
            }
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al consultar notificaciones no leídas: {ex.Message}", ex);
        }
    }

    private DataTable ObtenerNotificaciones(BsonDocument filtro)
    {
        try {
            var sort = Builders<BsonDocument>.Sort.Descending("FechaEnvio");
            var docs = _coleccion.Find(filtro).Sort(sort).ToList();
            return ConvertirATablaDeDatos(docs);
        }
        catch (Exception ex) { throw new ErrorRepository($"Error al consultar notificaciones: {ex.Message}", ex); }
    }

    private DataTable ConvertirATablaDeDatos(List<BsonDocument> docs)
    {
        var tabla = new DataTable();
        tabla.Columns.Add("id_notificacion", typeof(string));
        tabla.Columns.Add("carnet_usuario", typeof(string));
        tabla.Columns.Add("titulo", typeof(string));
        tabla.Columns.Add("contenido", typeof(string));
        tabla.Columns.Add("fecha_envio", typeof(DateTime));
        tabla.Columns.Add("leido", typeof(bool));
        foreach (var doc in docs)
        {
            var fila = tabla.NewRow();
            fila["id_notificacion"] = doc["_id"].ToString();
            fila["carnet_usuario"] = doc["CarnetUsuario"].AsString;
            fila["titulo"] = doc["Titulo"].AsString;
            fila["contenido"] = doc["Contenido"].AsString;
            fila["fecha_envio"] = doc["FechaEnvio"].ToUniversalTime();
            fila["leido"] = doc.GetValue("Leido", false).AsBoolean;
            tabla.Rows.Add(fila);
        }
        return tabla;
    }
}
