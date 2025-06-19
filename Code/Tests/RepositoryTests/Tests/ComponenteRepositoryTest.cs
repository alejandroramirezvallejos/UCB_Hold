using Moq;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class ComponenteRepositoryTest : IComponenteRepositoryTest
    {
        private Mock<IExecuteQuery>    _ejecutarConsultaMock;
        private IComponenteRepository _componenteRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock  = new Mock<IExecuteQuery>();
            _componenteRepositorio = new ComponenteRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearComponenteComando comando = new CrearComponenteComando("Nuevo Componente", "NC-01", "Tipo Nuevo", 5, "Desc Nuevo", 150.00, "http://example.com/nc01.pdf");
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
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "prueba componente actualizada", "prueba-v2", "jjjj", 7, "desc actualizada", 10.00, "http://example.com/updated.pdf");
            _componenteRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_componente")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 4;
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

            Assert.Throws<ErrorRepository>(() => _componenteRepositorio.Crear(new CrearComponenteComando("Test", "Test", "Test", 1, "Test", 1, "Test")));
            Assert.Throws<ErrorRepository>(() => _componenteRepositorio.Actualizar(new ActualizarComponenteComando(1, "Test", "Test", "Test", 1, "Test", 1, "Test")));
            Assert.Throws<ErrorRepository>(() => _componenteRepositorio.Eliminar(1));
        }
    }
}
