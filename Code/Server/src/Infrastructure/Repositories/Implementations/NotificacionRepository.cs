using MongoDB.Bson;
using MongoDB.Driver;
using IMT_Reservas.Server.Infrastructure.MongoDb;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class NotificacionRepository : INotificacionRepository
{
	private readonly IMongoCollection<BsonDocument> _coleccion;

	public NotificacionRepository(MongoDbContexto contexto)
		=> _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("notificaciones");

	public async Task<bool> ExisteActivoPorId(int id)
	{
		if (!ObjectId.TryParse(id.ToString(), out var objectId))
			return false;

		var filtro = Builders<BsonDocument>.Filter.And(
			Builders<BsonDocument>.Filter.Eq("_id", objectId),
			Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
		);
		var count = await _coleccion.CountDocumentsAsync(filtro);
		return count > 0;
	}
}
