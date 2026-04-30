using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class MantenimientoRepositoryTest 
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
            
            DataTable resDt = new DataTable();
            resDt.Columns.Add("id");
            resDt.Rows.Add(1);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(resDt);

            _mantenimientoRepositorio.CrearMantenimiento(1, comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarFuncion(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.AtLeastOnce);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _mantenimientoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 7;
            _mantenimientoRepositorio.Eliminar(new EliminarMantenimientoComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Exactly(2));
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _mantenimientoRepositorio.CrearMantenimiento(1, new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Test", 100, "desc", new int[] {1}, new string[] {"tipo"}, new string[] {"desc"})));
            Assert.Throws<Exception>(() => _mantenimientoRepositorio.Eliminar(new EliminarMantenimientoComando(1)));
        }
    }
}







