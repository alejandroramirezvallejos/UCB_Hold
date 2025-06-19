using Moq;
using System.Data;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class NotificacionRepositoryTest
    {
        private Mock<MongoDbContexto> _contextoMock;
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoCollection<BsonDocument>> _collectionMock;
        private NotificacionRepository _notificacionRepository;

        [SetUp]
        public void Setup()
        {
            var mockOptions = new Mock<IOptions<MongoDbConfiguracion>>();
            mockOptions.Setup(o => o.Value).Returns(new MongoDbConfiguracion { ConnectionString = "mongodb://localhost:27017", DatabaseName = "TestDb" });

            _contextoMock = new Mock<MongoDbContexto>(mockOptions.Object);
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<BsonDocument>>();

            _databaseMock.Setup(db => db.GetCollection<BsonDocument>("Notificaciones", null)).Returns(_collectionMock.Object);
            _contextoMock.Setup(c => c.BaseDeDatos).Returns(_databaseMock.Object);

            _notificacionRepository = new NotificacionRepository(_contextoMock.Object);
        }

        [Test]
        public void Crear_LlamaAInsertOne()
        {
            var comando = new CrearNotificacionComando("12890061", "Solicitud aprobada", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada. Pue…");
            
            _notificacionRepository.Crear(comando);

            _collectionMock.Verify(c => c.InsertOne(It.IsAny<BsonDocument>(), null, default), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaAUpdateOne()
        {
            var comando = new EliminarNotificacionComando("68535f7ddd47665ee70310b7", "12890061");
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>();
            var expectedUpdate = Builders<BsonDocument>.Update.Set("EstadoEliminado", true);
            var renderedExpected = expectedUpdate.Render(serializer, BsonSerializer.SerializerRegistry);

            _collectionMock.Setup(c => c.UpdateOne(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.Is<UpdateDefinition<BsonDocument>>(u => u.Render(serializer, BsonSerializer.SerializerRegistry).Equals(renderedExpected)),
                It.IsAny<UpdateOptions>(),
                default)).Verifiable();
            
            _notificacionRepository.Eliminar(comando);

            _collectionMock.Verify();
        }

        [Test]
        public void MarcarComoLeida_LlamaAUpdateOne()
        {
            var comando = new MarcarComoLeidoComando("68535f7ddd47665ee70310b7");
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>();
            var expectedUpdate = Builders<BsonDocument>.Update.Set("Leida", true);
            var renderedExpected = expectedUpdate.Render(serializer, BsonSerializer.SerializerRegistry);

            _collectionMock.Setup(c => c.UpdateOne(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.Is<UpdateDefinition<BsonDocument>>(u => u.Render(serializer, BsonSerializer.SerializerRegistry).Equals(renderedExpected)),
                It.IsAny<UpdateOptions>(),
                default)).Verifiable();
            
            _notificacionRepository.MarcarComoLeida(comando);

            _collectionMock.Verify();
        }

        [Test]
        public void ObtenerPorUsuario_RetornaDataTable()
        {
            var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta("12890061");
            var documentos = new List<BsonDocument>
            {
                new BsonDocument
                {
                    { "_id", new ObjectId("68535f7ddd47665ee70310b7") },
                    { "CarnetUsuario", "12890061" },
                    { "Titulo", "Solicitud aprobada" },
                    { "Contenido", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada. Pue…" },
                    { "FechaEnvio", DateTime.Parse("2025-06-12T09:15:00.000Z") },
                    { "Leida", false },
                    { "EstadoEliminado", false }
                },
                new BsonDocument
                {
                    { "_id", new ObjectId("68535f7ddd47665ee70310b8") },
                    { "CarnetUsuario", "12890061" },
                    { "Titulo", "Solicitud rechazada" },
                    { "Contenido", "Tu solicitud de préstamo para Monitor Profesional ha sido rechazada de…" },
                    { "FechaEnvio", DateTime.Parse("2025-06-14T10:30:00.000Z") },
                    { "Leida", false },
                    { "EstadoEliminado", false }
                }
            };

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.Setup(_ => _.Current).Returns(documentos);
            cursorMock.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            _collectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default))
                .Returns(cursorMock.Object);

            var resultado = _notificacionRepository.ObtenerPorUsuario(consulta);

            Assert.That(resultado, Is.InstanceOf<DataTable>());
            Assert.That(resultado.Rows.Count, Is.EqualTo(2));
            Assert.That(resultado.Rows[0]["id_notificacion"].ToString(), Is.EqualTo("68535f7ddd47665ee70310b7"));
            Assert.That(resultado.Rows[1]["id_notificacion"].ToString(), Is.EqualTo("68535f7ddd47665ee70310b8"));
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaErrorRepository()
        {
            var exception = new Exception("test exception");
            var validObjectId = ObjectId.GenerateNewId().ToString();

            _collectionMock.Setup(c => c.InsertOne(It.IsAny<BsonDocument>(), null, default)).Throws(exception);
            _collectionMock.Setup(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default)).Throws(exception);
            _collectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).Throws(exception);

            Assert.Throws<ErrorRepository>(() => _notificacionRepository.Crear(new CrearNotificacionComando("u", "t", "c")));
            Assert.Throws<ErrorRepository>(() => _notificacionRepository.Eliminar(new EliminarNotificacionComando(validObjectId, "u")));
            Assert.Throws<ErrorRepository>(() => _notificacionRepository.MarcarComoLeida(new MarcarComoLeidoComando(validObjectId)));
            Assert.Throws<ErrorRepository>(() => _notificacionRepository.ObtenerPorUsuario(new ObtenerNotificacionPorCarnetUsuarioConsulta("u")));
        }

        [Test]
        public void Repositorio_IdInvalido_LanzaErrorDataBase()
        {
            var invalidId = "id";
            Assert.Throws<ErrorDataBase>(() => _notificacionRepository.Eliminar(new EliminarNotificacionComando(invalidId, "u")));
            Assert.Throws<ErrorDataBase>(() => _notificacionRepository.MarcarComoLeida(new MarcarComoLeidoComando(invalidId)));
        }
    }
}
