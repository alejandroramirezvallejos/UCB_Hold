using IMT_Reservas.Server.Core.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.MongoDb;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComentarioRepository : IComentarioRepository
{
	private readonly IMongoCollection<BsonDocument> _coleccion;

	public ComentarioRepository(MongoDbContexto contexto)
		=> _coleccion = contexto.BaseDeDatos.GetCollection<BsonDocument>("comentarios");

	public async Task<bool> ExisteActivoPorId(string id)
	{
		if (!ObjectId.TryParse(id, out var objectId))
			return false;

		var filtro = Builders<BsonDocument>.Filter.And(
			Builders<BsonDocument>.Filter.Eq("_id", objectId),
			Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
		);
		var count = await _coleccion.CountDocumentsAsync(filtro);
		return count > 0;
	}
}
