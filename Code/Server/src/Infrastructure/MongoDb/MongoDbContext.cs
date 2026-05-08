using MongoDB.Driver;
using Microsoft.Extensions.Options;
using IMT_Reservas.Server.Core.Entities;

namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database = null!;

        public MongoDbContext(IOptions<MongoDbConfig> configuracion)
        {
            MongoClient clienteMongo = new MongoClient(configuracion.Value.ConnectionString);
            _database = clienteMongo.GetDatabase(configuracion.Value.DatabaseName);
        }

        public virtual IMongoDatabase Database => _database;

        public virtual IMongoCollection<Contrato> GetContratos => _database.GetCollection<Contrato>("contratos");
    }
}
