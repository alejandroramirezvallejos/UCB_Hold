using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace IMT_Reservas.Server.Core.Entities;

public class Contrato : Entity
{
    [BsonIgnore]
    public new int Id { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }

    [BsonElement("prestamoId")]
    public int PrestamoId { get; set; }
    [BsonElement("fileId")]
    public string? FileId { get; set; }
    public string? ContenidoBase64 { get; set; }
    public DateTime FechaCreacion { get; set; }
    [BsonElement("estadoEliminado")]
    public bool EstadoEliminado { get; set; }
}
