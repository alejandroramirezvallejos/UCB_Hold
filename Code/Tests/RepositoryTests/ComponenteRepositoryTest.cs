using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class ComponenteRepositoryTest
    {
        private Mock<ExecuteQuery>    _ejecutarConsultaMock;
        private IComponenteRepository _componenteRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock  = new Mock<ExecuteQuery>();
            _componenteRepositorio = new ComponenteRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearComponenteComando comando = new CrearComponenteComando("Test Componente", "Test Model", "Test Type", 123, "Test Desc", 99.99, "http://test.com");
            _componenteRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_componente")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_componentes")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _componenteRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "Updated Componente", "Updated Model", "Updated Type", 456, "Updated Desc", 199.99, "http://updated.com");
            _componenteRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_componente")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _componenteRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_componente")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _componenteRepositorio.Crear(new CrearComponenteComando("Test", null, null, 1, null, null, null)));
            Assert.Throws<Exception>(() => _componenteRepositorio.Actualizar(new ActualizarComponenteComando(1, "Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _componenteRepositorio.Eliminar(1));
        }
    }
}

