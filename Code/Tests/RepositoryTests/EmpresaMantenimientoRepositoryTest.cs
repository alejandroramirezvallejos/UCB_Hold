using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class EmpresaMantenimientoRepositoryTest
    {
        private Mock<ExecuteQuery>              _ejecutarConsultaMock;
        private IEmpresaMantenimientoRepository _empresaMantenimientoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock            = new Mock<ExecuteQuery>();
            _empresaMantenimientoRepositorio = new EmpresaMantenimientoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Tech Services", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");
            _empresaMantenimientoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_empresa_mantenimiento")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.NombreEmpresa)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_empresas_mantenimiento")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _empresaMantenimientoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "Tech Solutions", "Ana", "Gomez", "87654321", "Av. Siempreviva 742", "8765432-1");
            _empresaMantenimientoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_empresa_mantenimiento")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _empresaMantenimientoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_empresas_mantenimiento")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Crear(new CrearEmpresaMantenimientoComando("Test", null, null, null, null, null)));
            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Actualizar(new ActualizarEmpresaMantenimientoComando(1, "Test", null, null, null, null, null)));
            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Eliminar(1));
        }
    }
}

