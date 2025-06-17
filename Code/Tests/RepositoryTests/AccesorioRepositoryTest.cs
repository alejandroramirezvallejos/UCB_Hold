using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class AccesorioRepositoryTest
    {
        private Mock<ExecuteQuery>   _ejecutarConsultaMock;
        private IAccesorioRepository _accesorioRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<ExecuteQuery>();
            _accesorioRepositorio = new AccesorioRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Test Accesorio", "Test Model", "Test Type", 123, "Test Desc", 99.99, "http://test.com");
            _accesorioRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_accesorios")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_accesorios")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _accesorioRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(1, "Updated Accesorio", "Updated Model", "Updated Type", 456, "Updated Desc", 199.99, "http://updated.com");
            _accesorioRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_accesorio")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _accesorioRepositorio.Eliminar(id);
            
            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_accesorio")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _accesorioRepositorio.Crear(new CrearAccesorioComando("Test", null, null, 1, null, null, null)));
            Assert.Throws<Exception>(() => _accesorioRepositorio.Actualizar(new ActualizarAccesorioComando(1, "Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _accesorioRepositorio.Eliminar(1));
        }
    }
}
