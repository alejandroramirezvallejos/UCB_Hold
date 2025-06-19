using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class MantenimientoRepositoryTest : IMantenimientoRepositoryTest
    {
        private Mock<IExecuteQuery>       _ejecutarConsultaMock;
        private IMantenimientoRepository _mantenimientoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock     = new Mock<IExecuteQuery>();
            _mantenimientoRepositorio = new MantenimientoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 10), "Empresa Nueva", 300.00, "Mantenimiento de servidor", new int[] { 8 }, new string[] { "Preventivo" }, new string[] { "Servidor Rack" });
            _mantenimientoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_mantenimiento")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombreEmpresa"] == comando.NombreEmpresaMantenimiento)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_mantenimientos")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _mantenimientoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 7;
            _mantenimientoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_mantenimiento")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<ErrorRepository>(() => _mantenimientoRepositorio.Crear(new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Test", 100, "desc", new int[] {1}, new string[] {"tipo"}, new string[] {"desc"})));
            Assert.Throws<ErrorRepository>(() => _mantenimientoRepositorio.Eliminar(1));
        }
    }
}
