using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace IMT_Reservas.Server.Core.Entities;

public class Contrato
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? MongoId { get; set; }

    [BsonElement("prestamoId")]
    public int PrestamoId { get; set; }

    [BsonElement("fileId")]
    public string? FileId { get; set; }

    [BsonElement("contenidoBase64")]
    public string? ContenidoBase64 { get; set; }

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    [BsonElement("estadoEliminado")]
    public bool EstadoEliminado { get; set; }
}
