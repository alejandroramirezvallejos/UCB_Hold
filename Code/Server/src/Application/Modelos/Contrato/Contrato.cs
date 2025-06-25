using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public class Contrato
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("prestamoId")]
        public int PrestamoId { get; set; }

        [BsonElement("fileId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; }

        [BsonElement("EstadoEliminado")]
        public bool EstadoEliminado { get; set; } = false;
    }
}
