using Moq;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class GaveteroRepositoryTest : IGaveteroRepositoryTest
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
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", 50, 40, 20);
            _gaveteroRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_gavetero")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_gaveteros")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _gaveteroRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(1, "GAV-02", "Grande", "Mueble-B", 60, 50, 25);
            _gaveteroRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_gavetero")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _gaveteroRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_gavetero")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<ErrorRepository>(() => _gaveteroRepositorio.Crear(new CrearGaveteroComando("Test", null, null, null, null, null)));
            Assert.Throws<ErrorRepository>(() => _gaveteroRepositorio.Actualizar(new ActualizarGaveteroComando(1, "Test", null, null, null, null, null)));
            Assert.Throws<ErrorRepository>(() => _gaveteroRepositorio.Eliminar(1));
        }
    }
}
