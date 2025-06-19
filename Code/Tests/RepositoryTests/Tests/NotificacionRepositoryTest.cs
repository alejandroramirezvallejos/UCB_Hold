using Moq;
using System.Data;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System;

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
            _contextoMock = new Mock<MongoDbContexto>();
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
            
            _notificacionRepository.Eliminar(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.Is<UpdateDefinition<BsonDocument>>(u => u.ToString().Contains("\"$set\": {\"EstadoEliminado\": true}}")), It.IsAny<UpdateOptions>(), default), Times.Once);
        }

        [Test]
        public void MarcarComoLeida_LlamaAUpdateOne()
        {
            var comando = new MarcarComoLeidoComando("68535f7ddd47665ee70310b7");
            
            _notificacionRepository.MarcarComoLeida(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.Is<UpdateDefinition<BsonDocument>>(u => u.ToString().Contains("\"$set\": {\"Leida\": true}}")), It.IsAny<UpdateOptions>(), default), Times.Once);
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

            var findFluentMock = new Mock<IFindFluent<BsonDocument, BsonDocument>>();
            findFluentMock.Setup(f => f.Sort(It.IsAny<SortDefinition<BsonDocument>>())).Returns(findFluentMock.Object);
            findFluentMock.Setup(f => f.ToList(default)).Returns(documentos);
            _collectionMock.Setup(c => c.Find(It.IsAny<FilterDefinition<BsonDocument>>(), null)).Returns(findFluentMock.Object);

            var resultado = _notificacionRepository.ObtenerPorUsuario(consulta);

            Assert.That(resultado, Is.InstanceOf<DataTable>());
            Assert.That(resultado.Rows.Count, Is.EqualTo(2));
            Assert.That(resultado.Rows[0]["id_notificacion"].ToString(), Is.EqualTo("68535f7ddd47665ee70310b7"));
            Assert.That(resultado.Rows[1]["id_notificacion"].ToString(), Is.EqualTo("68535f7ddd47665ee70310b8"));
        }
    }
}

