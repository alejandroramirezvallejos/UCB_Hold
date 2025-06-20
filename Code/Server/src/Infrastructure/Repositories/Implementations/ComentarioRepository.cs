using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using IMT_Reservas.Server.Infrastructure.MongoDb;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComentarioRepository : IComentarioRepository
{
    private readonly IMongoCollection<BsonDocument> _coleccion;

    public ComentarioRepository(MongoDbContexto contexto)
    {
        _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("comentarios");
    }

    public void Crear(CrearComentarioComando comando)
    {
        var documento = new BsonDocument
        {
            { "CarnetUsuario", comando.CarnetUsuario },
            { "IdGrupoEquipo", comando.IdGrupoEquipo },
            { "Contenido", comando.Contenido },
            { "Likes", 0 },
            { "FechaCreacion", DateTime.UtcNow },
            { "EstadoEliminado", false }
        };

        try
        {
            _coleccion.InsertOne(documento);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al crear comentario: {ex.Message}", ex);
        }
    }
    
    public void Eliminar(EliminarComentarioComando comando)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(comando.Id) || comando.Id.Length != 24)
                throw new ErrorDataBase("ID de comentario inválido: debe tener 24 caracteres", null, null, null);

            ObjectId objectId;
            if (!ObjectId.TryParse(comando.Id, out objectId))
                throw new ErrorDataBase("ID de comentario inválido: formato incorrecto", null, null, null);

            var builder = Builders<BsonDocument>.Filter;
            var filtro = builder.And(
                builder.Eq("_id", objectId),
                builder.Eq("EstadoEliminado", false)
            );

            // Primero verificamos si el comentario existe
            var existe = _coleccion.CountDocuments(filtro) > 0;
            if (!existe)
            {
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            }

            var resultado = _coleccion.UpdateOne(
                filtro,
                Builders<BsonDocument>.Update.Set("EstadoEliminado", true)
            );

            if (resultado.MatchedCount == 0)
            {
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            }
        }
        catch (FormatException ex)
        {
            throw new ErrorDataBase($"ID de comentario inválido: {ex.Message}", null, null, ex);
        }
        catch (ErrorDataBase)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al eliminar comentario: {ex.Message}", ex);
        }
    }

    public void AgregarLike(AgregarLikeComentarioComando comando)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(comando.Id) || comando.Id.Length != 24)
                throw new ErrorDataBase("ID de comentario inválido: debe tener 24 caracteres", null, null, null);

            ObjectId objectId;
            if (!ObjectId.TryParse(comando.Id, out objectId))
                throw new ErrorDataBase("ID de comentario inválido: formato incorrecto", null, null, null);

            var builder = Builders<BsonDocument>.Filter;
            var filtro = builder.And(
                builder.Eq("_id", objectId),
                builder.Eq("EstadoEliminado", false)
            );

            // Primero verificamos si el comentario existe
            var existe = _coleccion.CountDocuments(filtro) > 0;
            if (!existe)
            {
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            }

            var resultado = _coleccion.UpdateOne(
                filtro,
                Builders<BsonDocument>.Update.Inc("Likes", 1)
            );

            if (resultado.MatchedCount == 0)
            {
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
            }
        }
        catch (FormatException ex)
        {
            throw new ErrorDataBase($"ID de comentario inválido: {ex.Message}", null, null, ex);
        }
        catch (ErrorDataBase)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al agregar like al comentario: {ex.Message}", ex);
        }
    }

    public DataTable ObtenerPorGrupoEquipo(int idGrupoEquipo)
    {
        try
        {
            var builder = Builders<BsonDocument>.Filter;
            var filtro = builder.And(
                builder.Eq("IdGrupoEquipo", idGrupoEquipo),
                builder.Eq("EstadoEliminado", false)
            );


            return ObtenerComentarios(filtro);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al consultar comentarios: {ex.Message}", ex);
        }
    }

    private DataTable ObtenerComentarios(FilterDefinition<BsonDocument> filtro)
    {
        try
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", filtro.Render(_coleccion.DocumentSerializer, _coleccion.Settings.SerializerRegistry)),
                new BsonDocument("$sort", new BsonDocument("FechaCreacion", -1)),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "usuarios" },
                    { "localField", "CarnetUsuario" },
                    { "foreignField", "Carnet" },
                    { "as", "usuario_info" }
                }),
                new BsonDocument("$unwind", new BsonDocument { { "path", "$usuario_info" }, { "preserveNullAndEmptyArrays", true } })
            };

            var cursor = _coleccion.Aggregate<BsonDocument>(pipeline);

            var documentos = cursor.ToList();

            return ConvertirATablaDeDatos(documentos);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error al consultar comentarios: {ex.Message}", ex);
        }
    }

    private DataTable ConvertirATablaDeDatos(List<BsonDocument> documentos)
    {
        var tabla = new DataTable();
        
        tabla.Columns.Add("id_comentario", typeof(string));
        tabla.Columns.Add("carnet_usuario", typeof(string));
        tabla.Columns.Add("nombre_usuario", typeof(string));
        tabla.Columns.Add("apellido_paterno_usuario", typeof(string));
        tabla.Columns.Add("id_grupo_equipo", typeof(int));
        tabla.Columns.Add("contenido_comentario", typeof(string));
        tabla.Columns.Add("likes_comentario", typeof(int));
        tabla.Columns.Add("fecha_creacion_comentario", typeof(DateTime));

        foreach (var doc in documentos)
        {
            var fila = tabla.NewRow();
            
            fila["id_comentario"] = doc["_id"].ToString();
            fila["carnet_usuario"] = doc["CarnetUsuario"].AsString;

            if (doc.Contains("usuario_info") && doc["usuario_info"] != BsonNull.Value)
            {
                var usuarioInfo = doc["usuario_info"].AsBsonDocument;
                fila["nombre_usuario"] = usuarioInfo.GetValue("Nombre", BsonNull.Value).AsString;
                fila["apellido_paterno_usuario"] = usuarioInfo.GetValue("ApellidoPaterno", BsonNull.Value).AsString;
            }
            else
            {
                fila["nombre_usuario"] = DBNull.Value;
                fila["apellido_paterno_usuario"] = DBNull.Value;
            }

            fila["id_grupo_equipo"] = doc["IdGrupoEquipo"].AsInt32;
            fila["contenido_comentario"] = doc["Contenido"].AsString;
            fila["likes_comentario"] = doc["Likes"].AsInt32;
            fila["fecha_creacion_comentario"] = doc["FechaCreacion"].ToUniversalTime();

            tabla.Rows.Add(fila);
        }
        return tabla;
    }
}