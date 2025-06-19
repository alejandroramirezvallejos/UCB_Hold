using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using IMT_Reservas.Server.Infrastructure.MongoDb;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly IMongoCollection<BsonDocument> _coleccion;

        public NotificacionRepository(MongoDbContexto contexto)
        {
            _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("Notificaciones");
        }

        public void Crear(CrearNotificacionComando comando)
        {
            var documento = new BsonDocument
            {
                { "CarnetUsuario", comando.CarnetUsuario },
                { "Titulo", comando.Titulo },
                { "Contenido", comando.Contenido },
                { "FechaEnvio", DateTime.UtcNow },
                { "Leida", false },
                { "EstadoEliminado", false }
            };

            try
            {
                _coleccion.InsertOne(documento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear la notificación: {ex.Message}", ex);
            }
        }

        public void Eliminar(EliminarNotificacionComando comando)
        {
            var filtro = new BsonDocument 
            {
                { "_id", new ObjectId(comando.Id) },
                { "CarnetUsuario", comando.CarnetUsuario }
            };
            var actualizacion = Builders<BsonDocument>.Update.Set("EstadoEliminado", true);

            try
            {
                _coleccion.UpdateOne(filtro, actualizacion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la notificación: {ex.Message}", ex);
            }
        }

        public DataTable ObtenerPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta)
        {
            var filtro = new BsonDocument
            {
                { "CarnetUsuario", consulta.CarnetUsuario },
                { "EstadoEliminado", false }
            };

            return ObtenerNotificaciones(filtro);
        }

        public void MarcarComoLeida(MarcarComoLeidoComando comando)
        {
            var filtro = new BsonDocument { { "_id", new ObjectId(comando.Id) } };
            var actualizacion = Builders<BsonDocument>.Update.Set("Leida", true);

            try
            {
                _coleccion.UpdateOne(filtro, actualizacion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar la notificación como leída: {ex.Message}", ex);
            }
        }

        private DataTable ObtenerNotificaciones(BsonDocument filtro)
        {
            try
            {
                var documentos = _coleccion.Find(filtro)
                    .Sort(new BsonDocument("FechaEnvio", -1))
                    .ToList();

                return ConvertirATablaDeDatos(documentos);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar notificaciones: {ex.Message}", ex);
            }
        }

        private DataTable ConvertirATablaDeDatos(List<BsonDocument> documentos)
        {
            var tabla = new DataTable();
            tabla.Columns.Add("id_notificacion", typeof(string));
            tabla.Columns.Add("carnet_usuario", typeof(string));
            tabla.Columns.Add("titulo", typeof(string));
            tabla.Columns.Add("contenido", typeof(string));
            tabla.Columns.Add("fecha_envio", typeof(DateTime));
            tabla.Columns.Add("leida", typeof(bool));

            foreach (var doc in documentos)
            {
                var fila = tabla.NewRow();
                fila["id_notificacion"] = doc["_id"].ToString();
                fila["carnet_usuario"] = doc["CarnetUsuario"].AsString;
                fila["titulo"] = doc["Titulo"].AsString;
                fila["contenido"] = doc["Contenido"].AsString;
                fila["fecha_envio"] = doc["FechaEnvio"].ToUniversalTime();
                fila["leida"] = doc["Leida"].AsBoolean;
                tabla.Rows.Add(fila);
            }
            return tabla;
        }
    }
}
