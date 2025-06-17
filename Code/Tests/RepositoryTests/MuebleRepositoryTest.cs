using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class MuebleRepositoryTest
    {
        private Mock<ExecuteQuery> _ejecutarConsultaMock;
        private IMuebleRepository  _muebleRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<ExecuteQuery>();
            _muebleRepositorio    = new MuebleRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);
            _muebleRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_mueble")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_muebles")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _muebleRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(1, "Mesa de Reuniones", "Oficina", 250.00, "Sala 2", 200.0, 100.0, 75.0);
            _muebleRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_mueble")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _muebleRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_mueble")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _muebleRepositorio.Crear(new CrearMuebleComando("Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _muebleRepositorio.Actualizar(new ActualizarMuebleComando(1, "Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _muebleRepositorio.Eliminar(1));
        }
    }
}

