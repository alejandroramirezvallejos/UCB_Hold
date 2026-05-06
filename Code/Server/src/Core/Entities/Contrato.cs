using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace IMT_Reservas.Server.Core.Entities;

public class Contrato : Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }

    public int PrestamoId { get; set; }
    public string? FileId { get; set; }
    public string? ContenidoBase64 { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoEliminado { get; set; }
}
