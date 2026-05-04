using Moq;
using System.Data;
using Ardalis.Result;
using MongoDB.Driver;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver.GridFS;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class PrestamoRepositoryTest 
    {
        private Mock<IExecuteQuery>  _ejecutarConsultaMock;
        private Mock<MongoDbContext> _mongoDbContextoMock;
        private Mock<IGridFSBucket> _gridFsBucketMock;
        private IPrestamoRepository _prestamoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _mongoDbContextoMock = new Mock<MongoDbContext>();
            _gridFsBucketMock = new Mock<IGridFSBucket>();
            _prestamoRepositorio  = new PrestamoRepository(_ejecutarConsultaMock.Object, _mongoDbContextoMock.Object, _gridFsBucketMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            var comando = new CrearPrestamoComando(new int[] { 1 }, System.DateTime.Now.AddDays(1), System.DateTime.Now.AddDays(2), "Obs", "12890061", null);
            var dt = new DataTable();
            dt.Columns.Add("id_prestamo", typeof(int));
            dt.Columns.Add("id_equipo", typeof(int));
            dt.Columns.Add("codigo_imt", typeof(string));
            dt.Columns.Add("codigo_serial", typeof(string));
            dt.Columns.Add("nombre", typeof(string));
            dt.Columns.Add("modelo", typeof(string));
            dt.Columns.Add("marca", typeof(string));
            dt.Columns.Add("id_grupo_equipo", typeof(int));
            dt.Rows.Add(123, 1, "C01", "S01", "PC", "M1", "Dell", 1);

            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()))
                .Returns(dt);

            var result = _prestamoRepositorio.CrearPrestamo(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarFuncion(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>())
            , Times.Once);

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _prestamoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 19;
            _prestamoRepositorio.Eliminar(new EliminarPrestamoComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Exactly(2));
        }

        [Test]
        public void ActualizarIdContrato_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int prestamoId = 1;
            string contratoId = "xyz";
            _prestamoRepositorio.ActualizarIdContrato(prestamoId, contratoId);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new System.Exception("test exception"));
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new System.Exception("test exception"));

            Assert.Throws<Exception>(() => _prestamoRepositorio.CrearPrestamo(new CrearPrestamoComando(new int[] { 1 }, System.DateTime.Now, System.DateTime.Now, "Test", "12890061", null)));
            Assert.Throws<Exception>(() => _prestamoRepositorio.Eliminar(new EliminarPrestamoComando(19)));
            Assert.Throws<Exception>(() => _prestamoRepositorio.ActualizarIdContrato(1, "contract-id"));
        }
    }
}







