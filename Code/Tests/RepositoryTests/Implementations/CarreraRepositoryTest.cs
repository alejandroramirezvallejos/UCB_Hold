using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class CarreraRepositoryTest 
    {
        private Mock<IExecuteQuery> _ejecutarConsultaMock;
        private ICarreraRepository _carreraRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _carreraRepositorio   = new CarreraRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Psicopedagogía");
            _carreraRepositorio.Crear(comando);

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

            DataTable resultado = _carreraRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(5, "Ingeniería Civil");
            
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _carreraRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 25;
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _carreraRepositorio.Eliminar(new EliminarCarreraComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            Assert.Throws<Exception>(() => _carreraRepositorio.Crear(new CrearCarreraComando("Psicopedagogía")));
            Assert.Throws<Exception>(() => _carreraRepositorio.Actualizar(new ActualizarCarreraComando(5, "Ingeniería Civil")));
            Assert.Throws<Exception>(() => _carreraRepositorio.Eliminar(new EliminarCarreraComando(25)));
        }
    }
}







