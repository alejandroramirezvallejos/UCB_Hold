using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class CategoriaServiceTest : ICategoriaServiceTest
    {
        private Mock<CategoriaRepository> _categoriaRepositoryMock;
        private CategoriaService          _categoriaService;

        [SetUp]
        public void Setup()
        {
            _categoriaRepositoryMock = new Mock<CategoriaRepository>();
            _categoriaService        = new CategoriaService(_categoriaRepositoryMock.Object);
        }

        [Test]
        public void CrearCategoria_ComandoValido_LlamaRepositorioCrear()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("Periféricos");
            _categoriaService.CrearCategoria(comando);
            _categoriaRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearCategoria_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("");
            Assert.Throws<ErrorNombreRequerido>(() => _categoriaService.CrearCategoria(comando));
        }

        [Test]
        public void CrearCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando(new string('a', 51));
            Assert.Throws<ErrorLongitudInvalida>(() => _categoriaService.CrearCategoria(comando));
        }

        [Test]
        public void ObtenerTodasCategorias_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable categoriasDataTable = new DataTable();
            categoriasDataTable.Columns.Add("id_categoria", typeof(int));
            categoriasDataTable.Columns.Add("categoria", typeof(string));
            categoriasDataTable.Rows.Add(1, "Periféricos");

            _categoriaRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(categoriasDataTable);

            List<CategoriaDto> resultado = _categoriaService.ObtenerTodasCategorias();

            Assert.That(resultado, Has.Exactly(1).Items.Matches<CategoriaDto>(categoria =>
                categoria.Id == 1 && categoria.Nombre == "Periféricos"
            ));
        }

        [Test]
        public void ActualizarCarrera_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(1, "Componentes PC");
            _categoriaService.ActualizarCategoria(comando);
            _categoriaRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void ActualizarCategoria_IdInvalido_LanzaErrorIdInvalido()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(0, "Componentes PC");
            Assert.Throws<ErrorIdInvalido>(() => _categoriaService.ActualizarCategoria(comando));
        }

        [Test]
        public void ActualizarCategoria_NombreVacio_LanzaErrorNombreRequerido()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(1, "");
            Assert.Throws<ErrorNombreRequerido>(() => _categoriaService.ActualizarCategoria(comando));
        }

        [Test]
        public void ActualizarCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(1, new string('a', 51));
            Assert.Throws<ErrorLongitudInvalida>(() => _categoriaService.ActualizarCategoria(comando));
        }

        [Test]
        public void EliminarCategoria_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarCategoriaComando comando = new EliminarCategoriaComando(1);
            _categoriaService.EliminarCategoria(comando);
            _categoriaRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarCategoria_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarCategoriaComando comando = new EliminarCategoriaComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _categoriaService.EliminarCategoria(comando));
        }
    }
}
