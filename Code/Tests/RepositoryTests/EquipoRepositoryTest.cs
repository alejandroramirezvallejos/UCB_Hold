using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class EquipoRepositoryTest
    {
        private Mock<ExecuteQuery> _ejecutarConsultaMock;
        private IEquipoRepository  _equipoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<ExecuteQuery>();
            _equipoRepositorio    = new EquipoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Laptop", "ThinkPad T480", "Lenovo", "UCB-LAP-01", "Laptop de alto rendimiento", "SN12345", "Oficina TI", "Compra directa", 1200.00, 5, "GAV-01");
            _equipoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.NombreGrupoEquipo)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_equipos")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _equipoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(1, "Laptop Gaming", "UCB-LAP-02", "desc act", "SN54321", "Oficina 2", "Donación", 1500.00, 3, "GAV-02", "En mantenimiento");
            _equipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _equipoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _equipoRepositorio.Crear(new CrearEquipoComando("Test", "Test", "Test", null, null, null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _equipoRepositorio.Actualizar(new ActualizarEquipoComando(1, "Test", null, null, null, null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _equipoRepositorio.Eliminar(1));
        }
    }
}

