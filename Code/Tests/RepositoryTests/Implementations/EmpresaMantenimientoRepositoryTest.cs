using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class EmpresaMantenimientoRepositoryTest 
    {
        private Mock<IExecuteQuery>              _ejecutarConsultaMock;
        private IEmpresaMantenimientoRepository _empresaMantenimientoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock            = new Mock<IExecuteQuery>();
            _empresaMantenimientoRepositorio = new EmpresaMantenimientoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Mantenimiento Global", "Carlos", "Ruiz", "55555555", "Av. Principal 456", "987654321");
            _empresaMantenimientoRepositorio.Crear(comando);

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

            DataTable resultado = _empresaMantenimientoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "JJJ Actualizado", "string", "string", "string", "string", "string");
            _empresaMantenimientoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 6;
            _empresaMantenimientoRepositorio.Eliminar(new EliminarEmpresaMantenimientoComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>() ))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Crear(new CrearEmpresaMantenimientoComando("a", "b", "c", "d", "e", "f")));
            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Actualizar(new ActualizarEmpresaMantenimientoComando(1, "a", "b", "c", "d", "e", "f")));
            Assert.Throws<Exception>(() => _empresaMantenimientoRepositorio.Eliminar(new EliminarEmpresaMantenimientoComando(6)));
        }
    }
}







