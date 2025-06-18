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
            MongoClient clienteMongo = new MongoClient(configuracion.Value.Conexion);
            _baseDeDatos             = clienteMongo.GetDatabase(configuracion.Value.BaseDeDatos);
            _gestionArchivos         = new GridFSBucket(_baseDeDatos);
        }

        public IMongoDatabase BaseDeDatos     => _baseDeDatos;

        public GridFSBucket   GestionArchivos => _gestionArchivos;
        
        public IMongoCollection<T> ObtenerColeccion<T>(string nombreColeccion)
        {
            return _baseDeDatos.GetCollection<T>(nombreColeccion);
        }
    }
}
