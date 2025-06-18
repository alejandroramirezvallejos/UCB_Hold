using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class CategoriaRepositoryTest : ICategoriaRepositoryTest
    {
        private Mock<ExecuteQuery>   _ejecutarConsultaMock;
        private ICategoriaRepository _categoriaRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<ExecuteQuery>();
            _categoriaRepositorio = new CategoriaRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("Adaptadores");
            _categoriaRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_categoria")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_categorias")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _categoriaRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(4, "Prueba Actualizada");
            _categoriaRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_categoria")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 10;
            _categoriaRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_categoria")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _categoriaRepositorio.Crear(new CrearCategoriaComando("Adaptadores")));
            Assert.Throws<Exception>(() => _categoriaRepositorio.Actualizar(new ActualizarCategoriaComando(4, "Prueba Actualizada")));
            Assert.Throws<Exception>(() => _categoriaRepositorio.Eliminar(10));
        }
    }
}
