using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.MongoDb;

public class ComentarioRepository : IComentarioRepository
{
    private readonly IMongoCollection<BsonDocument> _coleccion;
    public ComentarioRepository(MongoDbContexto contexto) => _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("comentarios");

    public Result<ComentarioDto> Crear(CrearComentarioComando comando)
    {
        var doc = new BsonDocument
        {
            ["CarnetUsuario"] = comando.CarnetUsuario,
            ["IdGrupoEquipo"] = comando.IdGrupoEquipo,
            ["Contenido"] = comando.Contenido,
            ["Likes"] = new BsonArray(),
            ["FechaCreacion"] = DateTime.UtcNow,
            ["EstadoEliminado"] = false
        };
        _coleccion.InsertOne(doc);
        var dto = new ComentarioDto { Contenido = comando.Contenido };
        return Result<ComentarioDto>.Created(dto);
    }

    public Result<ComentarioDto> Eliminar(EliminarComentarioComando comando)
    {
        if (!ObjectId.TryParse(comando.Id, out var objectId))
            return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", objectId),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        bool existe = _coleccion.CountDocuments(filtro) > 0;
        if (!existe) return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var res = _coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Set("EstadoEliminado", true));
        return res.MatchedCount == 0 ? Result<ComentarioDto>.NotFound("No se encontró el comentario") : Result<ComentarioDto>.Success(new ComentarioDto { Id = comando.Id });
    }

    public Result<ComentarioDto> AgregarLike(AgregarLikeComentarioComando comando)
    {
        if (!ObjectId.TryParse(comando.Id, out var objectId))
            return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", objectId),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        bool existe = _coleccion.CountDocuments(filtro) > 0;
        if (!existe) return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var yaDioLike = _coleccion.Find(
            Builders<BsonDocument>.Filter.And(
                filtro,
                Builders<BsonDocument>.Filter.ElemMatch("Likes", Builders<BsonDocument>.Filter.Eq("CarnetUsuario", comando.CarnetUsuario))
            )
        ).Any();
        if (yaDioLike) return Result<ComentarioDto>.Error("El usuario ya dio like a este comentario");
        var likeObj = new BsonDocument {
            { "CarnetUsuario", comando.CarnetUsuario },
            { "Fecha", DateTime.UtcNow }
        };
        var res = _coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Push("Likes", likeObj));
        return res.MatchedCount == 0 ? Result<ComentarioDto>.NotFound("No se encontró el comentario") : Result<ComentarioDto>.Success(new ComentarioDto { Id = comando.Id });
    }

    public Result<ComentarioDto> QuitarLike(QuitarLikeComentarioComando comando)
    {
        if (!ObjectId.TryParse(comando.Id, out var objectId))
            return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", objectId),
            Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
        );
        bool existe = _coleccion.CountDocuments(filtro) > 0;
        if (!existe) return Result<ComentarioDto>.NotFound("No se encontró el comentario");
        var update = Builders<BsonDocument>.Update.PullFilter("Likes", Builders<BsonDocument>.Filter.Eq("CarnetUsuario", comando.CarnetUsuario));
        var res = _coleccion.UpdateOne(filtro, update);
        return res.MatchedCount == 0 ? Result<ComentarioDto>.NotFound("No se encontró el comentario") : Result<ComentarioDto>.Success(new ComentarioDto { Id = comando.Id });
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
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", filtro.Render(_coleccion.DocumentSerializer, _coleccion.Settings.SerializerRegistry)),
            new BsonDocument("$sort", new BsonDocument("FechaCreacion", -1))
        };
        var docs = _coleccion.Aggregate<BsonDocument>(pipeline).ToList();
        return ConvertirATablaDeDatos(docs);
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