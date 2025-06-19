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
        _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("Comentarios");
    }

    public void Crear(CrearComentarioComando comando)
    {
        var documento = new BsonDocument
        {
            { "CarnetUsuario", comando.CarnetUsuario },
            { "IdGrupoEquipo", comando.IdGrupoEquipo.ToString() },
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
        var filtro = new BsonDocument
        {
            { "_id", new ObjectId(comando.Id) },
            { "EstadoEliminado", false }
        };

        try
        {
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
        var filtro = new BsonDocument
        {
            { "_id", new ObjectId(comando.Id) },
            { "EstadoEliminado", false }
        };

        try
        {
            var resultado = _coleccion.UpdateOne(
                filtro,
                Builders<BsonDocument>.Update.Inc("Likes", 1)
            );

            if (resultado.MatchedCount == 0)
                throw new ErrorDataBase("No se encontró el comentario", null, null, null);
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
            throw new ErrorRepository($"Error al agregar like: {ex.Message}", ex);
        }
    }

    public DataTable ObtenerPorGrupoEquipo(int idGrupoEquipo)
    {
        var builder = Builders<BsonDocument>.Filter;
        var filtro = builder.And(
            builder.Eq("IdGrupoEquipo", idGrupoEquipo.ToString()),
            builder.Eq("EstadoEliminado", false)
        );

        return ObtenerComentarios(filtro);
    }

    private DataTable ObtenerComentarios(FilterDefinition<BsonDocument> filtro)
    {
        try
        {
            var findOptions = new FindOptions<BsonDocument>
            {
                Sort = Builders<BsonDocument>.Sort.Descending("FechaCreacion")
            };
            var cursor = _coleccion.FindSync(filtro, findOptions);

            var documentos = new List<BsonDocument>();
            while (cursor.MoveNext())
            {
                documentos.AddRange(cursor.Current);
            }

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
        tabla.Columns.Add("id_grupo_equipo", typeof(string));
        tabla.Columns.Add("contenido_comentario", typeof(string));
        tabla.Columns.Add("likes_comentario", typeof(int));
        tabla.Columns.Add("fecha_creacion_comentario", typeof(DateTime));

        foreach (var doc in documentos)
        {
            var fila = tabla.NewRow();
            
            fila["id_comentario"] = doc["_id"].ToString();
            fila["carnet_usuario"] = doc["CarnetUsuario"].AsString;
            fila["nombre_usuario"] = DBNull.Value;
            fila["apellido_paterno_usuario"] = DBNull.Value;
            fila["id_grupo_equipo"] = doc["IdGrupoEquipo"].AsString;
            fila["contenido_comentario"] = doc["Contenido"].AsString;
            fila["likes_comentario"] = doc["Likes"].AsInt32;
            fila["fecha_creacion_comentario"] = doc["FechaCreacion"].ToUniversalTime();

            tabla.Rows.Add(fila);
        }
        return tabla;
    }
}