using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class CarreraRepositoryTest : ICarreraRepositoryTest
    {
        private Mock<ExecuteQuery> _ejecutarConsultaMock;
        private ICarreraRepository _carreraRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<ExecuteQuery>();
            _carreraRepositorio   = new CarreraRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Psicopedagogía");
            _carreraRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_carrera")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodas_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_carreras")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _carreraRepositorio.ObtenerTodas();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(5, "Ingeniería Civil");
            _carreraRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_carrera")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 25;
            _carreraRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_carrera")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _carreraRepositorio.Crear(new CrearCarreraComando("Psicopedagogía")));
            Assert.Throws<Exception>(() => _carreraRepositorio.Actualizar(new ActualizarCarreraComando(5, "Ingeniería Civil")));
            Assert.Throws<Exception>(() => _carreraRepositorio.Eliminar(25));
        }
    }
}
