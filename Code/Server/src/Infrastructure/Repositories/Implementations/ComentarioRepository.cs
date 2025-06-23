using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using IMT_Reservas.Server.Infrastructure.MongoDb;

public class ComentarioRepository : IComentarioRepository
{
    private readonly IMongoCollection<BsonDocument> _coleccion;
    public ComentarioRepository(MongoDbContexto contexto) => _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("comentarios");

    public void Crear(CrearComentarioComando comando)
    {
        var doc = new BsonDocument
        {
            ["CarnetUsuario"] = comando.CarnetUsuario,
            ["IdGrupoEquipo"] = comando.IdGrupoEquipo,
            ["Contenido"] = comando.Contenido,
            ["Likes"] = new BsonArray(), // Ahora es un array de objetos
            ["FechaCreacion"] = DateTime.UtcNow,
            ["EstadoEliminado"] = false
        };
        try { _coleccion.InsertOne(doc); }
        catch (Exception ex) { throw new ErrorRepository($"Error al crear comentario: {ex.Message}", ex); }
    }

    public void Eliminar(EliminarComentarioComando comando)
    {
        if (string.IsNullOrWhiteSpace(comando.Id) || comando.Id.Length != 24 || !ObjectId.TryParse(comando.Id, out var objectId))
            throw new ErrorDataBase("ID de comentario inválido", null, null, null);
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", objectId),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        try {
            if (_coleccion == null)
                throw new ErrorRepository("No se encontró el comentario", null);
            // Si ocurre cualquier excepción en CountDocuments, captúrala y relanza como ErrorRepository
            bool existe;
            try {
                existe = _coleccion.CountDocuments(filtro) > 0;
            } catch (Exception ex) {
                throw new ErrorRepository($"Error al verificar existencia del comentario: {ex.Message}", ex);
            }
            if (!existe) throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            var res = _coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Set("EstadoEliminado", true));
            if (res == null || res.MatchedCount == 0) throw new ErrorDataBase("No se encontró el comentario", null, null, null);
        }
        catch (ErrorDataBase) { throw; }
        catch (Exception ex) { throw new ErrorRepository($"Error al eliminar comentario: {ex.Message}", ex); }
    }

    public void AgregarLike(AgregarLikeComentarioComando comando)
    {
        if (string.IsNullOrWhiteSpace(comando.Id) || comando.Id.Length != 24 || !ObjectId.TryParse(comando.Id, out var objectId))
            throw new ErrorDataBase("ID de comentario inválido", null, null, null);
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", objectId),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        try {
            if (_coleccion == null)
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            bool existe;
            try {
                existe = _coleccion.CountDocuments(filtro) > 0;
            } catch (Exception ex) {
                throw new ErrorRepository($"Error al verificar existencia del comentario: {ex.Message}", ex);
            }
            if (!existe) throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            // Verificar si el usuario ya dio like
            var yaDioLike = _coleccion.Find(
                Builders<BsonDocument>.Filter.And(
                    filtro,
                    Builders<BsonDocument>.Filter.ElemMatch("Likes", Builders<BsonDocument>.Filter.Eq("CarnetUsuario", comando.CarnetUsuario))
                )
            ).Any();
            if (yaDioLike)
                throw new ErrorRepository("El usuario ya dio like a este comentario", null);
            var likeObj = new BsonDocument {
                { "CarnetUsuario", comando.CarnetUsuario },
                { "Fecha", DateTime.UtcNow }
            };
            var res = _coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Push("Likes", likeObj));
            if (res == null || res.MatchedCount == 0) throw new ErrorDataBase("No se encontró el comentario", null, null, null);
        }
        catch (ErrorDataBase) { throw; }
        catch (Exception ex) { throw new ErrorRepository($"Error al agregar like al comentario: {ex.Message}", ex); }
    }

    public DataTable ObtenerPorGrupoEquipo(int idGrupoEquipo)
    {
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("IdGrupoEquipo", idGrupoEquipo),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        return ObtenerComentarios(filtro);
    }

    private DataTable ObtenerComentarios(FilterDefinition<BsonDocument> filtro)
    {
        try {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", filtro.Render(_coleccion.DocumentSerializer, _coleccion.Settings.SerializerRegistry)),
                new BsonDocument("$sort", new BsonDocument("FechaCreacion", -1))
            };
            var docs = _coleccion.Aggregate<BsonDocument>(pipeline).ToList();
            return ConvertirATablaDeDatos(docs);
        }
        catch (Exception ex) { throw new ErrorRepository($"Error al consultar comentarios: {ex.Message}", ex); }
    }

    private DataTable ConvertirATablaDeDatos(List<BsonDocument> docs)
    {
        var tabla = new DataTable();
        tabla.Columns.Add("id_comentario", typeof(string));
        tabla.Columns.Add("carnet_usuario", typeof(string));
        tabla.Columns.Add("id_grupo_equipo", typeof(int));
        tabla.Columns.Add("contenido_comentario", typeof(string));
        tabla.Columns.Add("likes_comentario", typeof(int));
        tabla.Columns.Add("fecha_creacion_comentario", typeof(DateTime));
        foreach (var doc in docs)
        {
            var fila = tabla.NewRow();
            fila["id_comentario"] = doc["_id"].ToString();
            fila["carnet_usuario"] = doc["CarnetUsuario"].AsString;
            fila["id_grupo_equipo"] = doc["IdGrupoEquipo"].AsInt32;
            fila["contenido_comentario"] = doc["Contenido"].AsString;
            fila["likes_comentario"] = doc.Contains("Likes") && doc["Likes"].IsBsonArray ? doc["Likes"].AsBsonArray.Count : 0;
            fila["fecha_creacion_comentario"] = doc["FechaCreacion"].ToUniversalTime();

            tabla.Rows.Add(fila);
        }
        return tabla;
    }
}