using IMT_Reservas.Server.Core.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using IMT_Reservas.Server.Application.Features.Comentario.Dtos;

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

	public async Task<Result<ComentarioListDto>> CreateAsync(Dictionary<string, object?> parameters)
		=> Result<ComentarioListDto>.Error("MongoDB CreateAsync not implemented");

	public async Task<Result<ComentarioListDto>> UpdateAsync(Dictionary<string, object?> parameters)
		=> Result<ComentarioListDto>.Error("MongoDB UpdateAsync not implemented");

	public async Task<Result<List<ComentarioListDto>>> GetAllAsync(IMT_Reservas.Server.Core.Abstractions.QueryFilter? filter = null)
		=> Result<List<ComentarioListDto>>.Error("MongoDB GetAllAsync not implemented");

	public async Task<Result<ComentarioListDto>> GetByIdAsync(int id)
		=> Result<ComentarioListDto>.Error("MongoDB GetByIdAsync not implemented");

	public async Task<Result<object>> DeleteAsync(int id)
		=> Result<object>.Error("MongoDB DeleteAsync not implemented");
}
