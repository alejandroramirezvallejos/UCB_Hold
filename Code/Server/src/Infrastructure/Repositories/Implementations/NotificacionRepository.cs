using MongoDB.Bson;
using MongoDB.Driver;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Notificacion.Dtos;

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

	public async Task<Result<NotificacionListDto>> CreateAsync(Dictionary<string, object?> parameters)
		=> Result<NotificacionListDto>.Error("MongoDB CreateAsync not implemented");

	public async Task<Result<NotificacionListDto>> UpdateAsync(Dictionary<string, object?> parameters)
		=> Result<NotificacionListDto>.Error("MongoDB UpdateAsync not implemented");

	public async Task<Result<List<NotificacionListDto>>> GetAllAsync(IMT_Reservas.Server.Core.Abstractions.QueryFilter? filter = null)
		=> Result<List<NotificacionListDto>>.Error("MongoDB GetAllAsync not implemented");

	public async Task<Result<NotificacionListDto>> GetByIdAsync(int id)
		=> Result<NotificacionListDto>.Error("MongoDB GetByIdAsync not implemented");

	public async Task<Result<object>> DeleteAsync(int id)
		=> Result<object>.Error("MongoDB DeleteAsync not implemented");
}
