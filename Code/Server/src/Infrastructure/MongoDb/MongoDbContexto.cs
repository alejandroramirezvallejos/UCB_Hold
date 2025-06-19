using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Options;

namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public class MongoDbContexto
    {
        private readonly IMongoDatabase _baseDeDatos;
        private readonly GridFSBucket   _gestionArchivos;

        public MongoDbContexto(IOptions<MongoDbConfiguracion> configuracion)
        {
            MongoClient clienteMongo = new MongoClient(configuracion.Value.ConnectionString);
            _baseDeDatos             = clienteMongo.GetDatabase(configuracion.Value.DatabaseName);
            _gestionArchivos         = new GridFSBucket(_baseDeDatos);
        }

        public virtual IMongoDatabase BaseDeDatos     => _baseDeDatos;

        public virtual IGridFSBucket   GestionArchivos => _gestionArchivos;
        
        public virtual IMongoCollection<T> ObtenerColeccion<T>(string nombreColeccion)
        {
            return _baseDeDatos.GetCollection<T>(nombreColeccion);
        }
    }
}
