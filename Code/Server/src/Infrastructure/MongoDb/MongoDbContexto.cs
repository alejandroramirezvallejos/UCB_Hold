using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Options;
using IMT_Reservas.Server.Core.Entities;

namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public class MongoDbContexto
    {
        private readonly IMongoDatabase _baseDeDatos = null!;

        public MongoDbContexto(IOptions<MongoDbConfiguracion> configuracion)
        {
            MongoClient clienteMongo = new MongoClient(configuracion.Value.ConnectionString);
            _baseDeDatos = clienteMongo.GetDatabase(configuracion.Value.DatabaseName);
        }

        protected MongoDbContexto() { }

        public virtual IMongoDatabase BaseDeDatos => _baseDeDatos;
        
        public virtual IMongoCollection<Contrato> GetContratos => _baseDeDatos.GetCollection<Contrato>("contratos");
    }
}
