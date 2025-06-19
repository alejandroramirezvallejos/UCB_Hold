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
    public class ComentarioRepositoryTest
    {
        private Mock<MongoDbContexto> _contextoMock;
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoCollection<BsonDocument>> _collectionMock;
        private ComentarioRepository _comentarioRepository;

        [SetUp]
        public void Setup()
        {
            _contextoMock = new Mock<MongoDbContexto>();
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<BsonDocument>>();

            _databaseMock.Setup(db => db.GetCollection<BsonDocument>("Comentarios", null)).Returns(_collectionMock.Object);
            _contextoMock.Setup(c => c.BaseDeDatos).Returns(_databaseMock.Object);

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
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(1);
            _collectionMock.Setup(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default)).Returns(mockUpdateResult.Object);

            _comentarioRepository.Eliminar(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.Is<UpdateDefinition<BsonDocument>>(u => u.ToString().Contains("\"$set\": {\"EstadoEliminado\": true}}")), It.IsAny<UpdateOptions>(), default), Times.Once);
        }
        
        [Test]
        public void Eliminar_IdNoExistente_LanzaErrorDataBase()
        {
            var comando = new EliminarComentarioComando("000000000000000000000000");
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(0);
            _collectionMock.Setup(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default)).Returns(mockUpdateResult.Object);

            Assert.Throws<ErrorDataBase>(() => _comentarioRepository.Eliminar(comando));
        }

        [Test]
        public void AgregarLike_LlamaAUpdateOne()
        {
            var comando = new AgregarLikeComentarioComando("68531f233cba0b4adf2ea2cc");
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(1);
            _collectionMock.Setup(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default)).Returns(mockUpdateResult.Object);

            _comentarioRepository.AgregarLike(comando);

            _collectionMock.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.Is<UpdateDefinition<BsonDocument>>(u => u.ToString().Contains("\"$inc\": {\"Likes\": 1}}")), It.IsAny<UpdateOptions>(), default), Times.Once);
        }
        
        [Test]
        public void AgregarLike_IdNoExistente_LanzaErrorDataBase()
        {
            var comando = new AgregarLikeComentarioComando("000000000000000000000000");
            var mockUpdateResult = new Mock<UpdateResult>();
            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(0);
            _collectionMock.Setup(c => c.UpdateOne(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), It.IsAny<UpdateOptions>(), default)).Returns(mockUpdateResult.Object);

            Assert.Throws<ErrorDataBase>(() => _comentarioRepository.AgregarLike(comando));
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
                    { "IdGrupoEquipo", "8" },
                    { "Contenido", "El servidor está bien configurado, pero recomendaría actualizar el sis…" },
                    { "Likes", 3 },
                    { "FechaCreacion", DateTime.Parse("2025-06-12T09:15:00.000Z") },
                    { "EstadoEliminado", false }
                }
            };
            
            var findFluentMock = new Mock<IFindFluent<BsonDocument, BsonDocument>>();
            findFluentMock.Setup(f => f.Sort(It.IsAny<SortDefinition<BsonDocument>>())).Returns(findFluentMock.Object);
            findFluentMock.Setup(f => f.ToList(default)).Returns(documentos);
            _collectionMock.Setup(c => c.Find(It.IsAny<FilterDefinition<BsonDocument>>(), null)).Returns(findFluentMock.Object);

            var resultado = _comentarioRepository.ObtenerPorGrupoEquipo(idGrupoEquipo);

            Assert.That(resultado, Is.InstanceOf<DataTable>());
            Assert.That(resultado.Rows.Count, Is.EqualTo(1));
            Assert.That(resultado.Rows[0]["id_comentario"].ToString(), Is.EqualTo("68531f233cba0b4adf2ea2cd"));
        }
    }
}

