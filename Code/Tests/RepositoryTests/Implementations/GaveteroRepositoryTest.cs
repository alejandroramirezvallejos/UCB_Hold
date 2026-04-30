using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class GaveteroRepositoryTest 
    {
        private Mock<IExecuteQuery>  _ejecutarConsultaMock;
        private IGaveteroRepository _gaveteroRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _gaveteroRepositorio  = new GaveteroRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int idMueble = 1;
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", 50, 40, 20);
            _gaveteroRepositorio.Crear(idMueble, comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            var resultado = _gaveteroRepositorio.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.AreSame(tablaEsperada, resultado.Value);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(1, "GAV-02", "Grande", "Mueble-B", 60, 50, 25);
            
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _gaveteroRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _gaveteroRepositorio.Eliminar(new EliminarGaveteroComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>() ))
                           .Throws(new Exception("test exception"));

            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            Assert.Throws<Exception>(() => _gaveteroRepositorio.Crear(1, new CrearGaveteroComando("a", "b", "c", 1, 1, 1)));
            Assert.Throws<Exception>(() => _gaveteroRepositorio.Actualizar(1, new ActualizarGaveteroComando(1, "a", "b", "c", 1, 1, 1)));
            Assert.Throws<Exception>(() => _gaveteroRepositorio.Eliminar(new EliminarGaveteroComando(1)));
        }
    }
}







