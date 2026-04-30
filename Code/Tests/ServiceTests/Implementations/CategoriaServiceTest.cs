using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class CategoriaServiceTest
    {
        private Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private CategoriaService _categoriaService;

        [SetUp]
        public void Setup()
        {
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _categoriaService = new CategoriaService(_categoriaRepositoryMock.Object);
        }

        [Test]
        public void CrearCategoria_ComandoValido_LlamaRepositorioCrear()
        {
            var comando = new CrearCategoriaComando("Periféricos");
            _categoriaRepositoryMock.Setup(r => r.ReactivarEliminadaPorNombre(It.IsAny<string>())).Returns(false);
            _categoriaRepositoryMock.Setup(r => r.ExisteActivaPorNombre(It.IsAny<string>())).Returns(false);
            _categoriaRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearCategoriaComando>())).Returns(Result<CategoriaDto>.Created(new CategoriaDto { Id = 1, Nombre = "Periféricos" }));
            _categoriaService.Crear(comando);
            _categoriaRepositoryMock.Verify(r => r.Crear(It.IsAny<CrearCategoriaComando>()), Times.Once);
        }

        [Test]
        public void CrearCategoria_NombreVacio_LanzaErrorNombreRequerido()
        {
            var comando = new CrearCategoriaComando("");
            var resultado = _categoriaService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            var comando = new CrearCategoriaComando(new string('a', 256));
            var resultado = _categoriaService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ObtenerTodasCategorias_CuandoHayDatos_RetornaListaDeDtos()
        {
            var categoriasDataTable = new DataTable();
            categoriasDataTable.Columns.Add("id_categoria", typeof(int));
            categoriasDataTable.Columns.Add("categoria", typeof(string));
            categoriasDataTable.Rows.Add(1, "Periféricos");
            _categoriaRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(categoriasDataTable));
            var resultado = _categoriaService.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Exactly(1).Items.Matches<CategoriaDto>(c =>
                c.Id == 1 && c.Nombre == "Periféricos"
            ));
        }

        [Test]
        public void ActualizarCarrera_ComandoValido_LlamaRepositorioActualizar()
        {
            var comando = new ActualizarCategoriaComando(1, "Componentes PC");
            _categoriaRepositoryMock.Setup(r => r.ExisteActivaPorId(1)).Returns(true);
            _categoriaRepositoryMock.Setup(r => r.ExisteActivaPorNombreExcluyendoId(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            _categoriaRepositoryMock.Setup(r => r.ReactivarEliminadaPorNombre(It.IsAny<string>())).Returns(false);
            _categoriaRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.Success(new CategoriaDto { Id = 1, Nombre = "Componentes PC" }));
            _categoriaService.Actualizar(comando);
            _categoriaRepositoryMock.Verify(r => r.Actualizar(It.IsAny<ActualizarCategoriaComando>()), Times.Once);
        }

        [Test]
        public void ActualizarCategoria_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new ActualizarCategoriaComando(0, "Componentes PC");
            var resultado = _categoriaService.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarCategoria_NombreVacio_LanzaErrorNombreRequerido()
        {
            var comando = new ActualizarCategoriaComando(1, "");
            _categoriaRepositoryMock.Setup(r => r.ExisteActivaPorId(1)).Returns(true);
            var resultado = _categoriaService.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            var comando = new ActualizarCategoriaComando(1, new string('a', 256));
            var resultado = _categoriaService.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarCategoria_ComandoValido_LlamaRepositorioEliminar()
        {
            var comando = new EliminarCategoriaComando(1);
            _categoriaRepositoryMock.Setup(r => r.ExisteActivaPorId(1)).Returns(true);
            _categoriaRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarCategoriaComando>())).Returns(Result<CategoriaDto>.Success(new CategoriaDto()));
            _categoriaService.Eliminar(comando);
            _categoriaRepositoryMock.Verify(r => r.Eliminar(It.IsAny<EliminarCategoriaComando>()), Times.Once);
        }

        [Test]
        public void EliminarCategoria_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new EliminarCategoriaComando(0);
            var resultado = _categoriaService.Eliminar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }
    }
}
