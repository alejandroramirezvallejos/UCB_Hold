using Moq;
using System.Data;
using Ardalis.Result;
using MongoDB.Driver;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Bson;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class ComentarioRepositoryTest
    {
        private Mock<MongoDbContext> _contextoMock;
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoCollection<BsonDocument>> _collectionMock;
        private ComentarioRepository _comentarioRepository;

        [SetUp]
        public void Setup()
        {
            var mockOptions = new Mock<IOptions<MongoDbConfig>>();
            mockOptions.Setup(o => o.Value).Returns(new MongoDbConfig { ConnectionString = "mongodb://localhost:27017", DatabaseName = "TestDb" });

            _contextoMock = new Mock<MongoDbContext>(mockOptions.Object);
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<BsonDocument>>();

            _databaseMock.Setup(db => db.GetCollection<BsonDocument>("comentarios", null)).Returns(_collectionMock.Object);
            _contextoMock.Setup(c => c.Database).Returns(_databaseMock.Object);

            _collectionMock.Setup(c => c.DocumentSerializer).Returns(BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>());
            _collectionMock.Setup(c => c.Settings).Returns(new MongoCollectionSettings());

            _comentarioRepository = new ComentarioRepository(_contextoMock.Object);
        }

        [Test]
        public void Crear_LlamaAInsertOne()
        {
            var comando = new CrearComentarioComando("2", 6, "El router funciona perfectamente, buena velocidad de conexión.");
            
            _comentarioRepository.Crear(comando);

            _collectionMock.Verify(c => c.InsertOne(It.IsAny<BsonDocument>(), null, default), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaAUpdateOne()
        {
            var comando = new EliminarComentarioComando("68531f233cba0b4adf2ea2cc");

            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(1L);

            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.Setup(r => r.MatchedCount).Returns(1);
            _collectionMock.Setup(c => c.UpdateOne(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(mockUpdateResult.Object);

            _comentarioRepository.Eliminar(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default), Times.Once);
        }
        
        [Test]
        public void Eliminar_IdNoExistente_LanzaErrorDataBase()
        {
            var comando = new EliminarComentarioComando("000000000000000000000000");
            
            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(0L);

            var result = _comentarioRepository.Eliminar(comando);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.NotFound));
            Assert.That(result.Errors.ElementAt(0), Is.EqualTo("No se encontró el comentario"));
        }

        [Test]
        public void AgregarLike_LlamaAUpdateOne()
        {
            var comando = new AgregarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "2");
            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(1L);
            var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
            mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<System.Threading.CancellationToken>())).Returns(false);
            mockCursor.Setup(x => x.Current).Returns(new List<BsonDocument>());
            _collectionMock.Setup(c => c.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<System.Threading.CancellationToken>()
            )).Returns(mockCursor.Object);
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.Setup(r => r.MatchedCount).Returns(1);
            _collectionMock.Setup(c => c.UpdateOne(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(mockUpdateResult.Object);
            _comentarioRepository.AgregarLike(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default), Times.Once);
        }
        
        [Test]
        public void AgregarLike_IdNoExistente_LanzaErrorDataBase()
        {
            var comando = new AgregarLikeComentarioComando("000000000000000000000000", "2");

            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(0L);

            var result = _comentarioRepository.AgregarLike(comando);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.NotFound));
            Assert.That(result.Errors.ElementAt(0), Is.EqualTo("No se encontró el comentario"));
        }

        [Test]
        public void ObtenerPorGrupoEquipo_RetornaDataTable()
        {
            var idGrupoEquipo = 8;
            var documentos = new List<BsonDocument>
            {
                new BsonDocument
                {
                    { "_id", new ObjectId("68531f233cba0b4adf2ea2cd") },
                    { "CarnetUsuario", "7" },
                    { "IdGrupoEquipo", idGrupoEquipo },
                    { "Contenido", "El servidor está bien configurado, pero recomendaría actualizar el sis…" },
                    { "Likes", 3 },
                    { "FechaCreacion", DateTime.Parse("2025-06-12T09:15:00.000Z") },
                    { "EstadoEliminado", false }
                }
            };
            
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.Setup(_ => _.Current).Returns(documentos);
            cursorMock.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            _collectionMock.Setup(c => c.Aggregate(It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(), It.IsAny<AggregateOptions>(), default))
                .Returns(cursorMock.Object);

            var resultado = _comentarioRepository.ObtenerPorGrupoEquipo(idGrupoEquipo);

            Assert.That(resultado, Is.InstanceOf<DataTable>());
            Assert.That(resultado.Rows.Count, Is.EqualTo(1));
            Assert.That(resultado.Rows[0]["id_comentario"].ToString(), Is.EqualTo("68531f233cba0b4adf2ea2cd"));
            Assert.That(resultado.Rows[0]["carnet_usuario"].ToString(), Is.EqualTo("7"));
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaErrorRepository()
        {
            var objectId = "68531f233cba0b4adf2ea2cc";
            var exception = new Exception("test exception");

            _collectionMock.Setup(c => c.InsertOne(It.IsAny<BsonDocument>(), null, default)).Throws(exception);
            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Throws(exception);
            _collectionMock.Setup(c => c.Aggregate(It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(), It.IsAny<AggregateOptions>(), default)).Throws(exception);

            Assert.Throws<Exception>(() => _comentarioRepository.Crear(new CrearComentarioComando("1", 1, "test")));
            Assert.Throws<Exception>(() => _comentarioRepository.Eliminar(new EliminarComentarioComando(objectId)));
            Assert.Throws<Exception>(() => _comentarioRepository.AgregarLike(new AgregarLikeComentarioComando(objectId, "2")));
            Assert.Throws<Exception>(() => _comentarioRepository.ObtenerPorGrupoEquipo(1));
        }

        [Test]
        public void QuitarLike_LlamaAUpdateOne()
        {
            var comando = new QuitarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "2");
            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(1L);
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.Setup(r => r.MatchedCount).Returns(1);
            _collectionMock.Setup(c => c.UpdateOne(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(mockUpdateResult.Object);
            _comentarioRepository.QuitarLike(comando);
            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default), Times.Once);
        }

        [Test]
        public void QuitarLike_IdNoExistente_LanzaErrorDataBase()
        {
            var comando = new QuitarLikeComentarioComando("000000000000000000000000", "2");
            _collectionMock.Setup(c => c.CountDocuments(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<CountOptions>(), default)).Returns(0L);
            var result = _comentarioRepository.QuitarLike(comando);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.NotFound));
            Assert.That(result.Errors.ElementAt(0), Is.EqualTo("No se encontró el comentario"));
        }
    }
}






