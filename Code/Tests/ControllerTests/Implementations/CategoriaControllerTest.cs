using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class CategoriaControllerTest
    {
        private Mock<ICategoriaService> _categoriaServiceMock;
        private CategoriaController _categoriasController;

        [SetUp]
        public void Setup()
        {
            _categoriaServiceMock = new Mock<ICategoriaService>();
            _categoriasController = new CategoriaController(_categoriaServiceMock.Object);
        }

        [Test]
        public void GetCategorias_ConDatos_RetornaOk()
        {
            var categoriasEsperadas = new List<CategoriaDto>
            {
                new CategoriaDto { Id = 1, Nombre = "Impresora" },
                new CategoriaDto { Id = 3, Nombre = "Cable" }
            };
            _categoriaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CategoriaDto>>.Success(categoriasEsperadas));
            var resultado = _categoriasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(categoriasEsperadas.Count));
        }

        [Test]
        public void GetCategorias_SinDatos_RetornaOkVacia()
        {
            _categoriaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CategoriaDto>>.Success(new List<CategoriaDto>()));
            var resultado = _categoriasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetCategorias_ServicioError_RetornaBadRequest()
        {
            _categoriaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CategoriaDto>>.Error("Error servicio"));
            var resultado = _categoriasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearCategoria_Valida_RetornaCreated()
        {
            var comando = new CrearCategoriaComando("Adaptadores");
            var dto = new CategoriaDto { Id = 1, Nombre = "Adaptadores" };
            _categoriaServiceMock.Setup(s => s.Crear(It.IsAny<CrearCategoriaComando>())).Returns(Result<CategoriaDto>.Created(dto));
            var resultado = _categoriasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearCategoria_BadRequest()
        {
            yield return new object[] { new CrearCategoriaComando(""), new ErrorNombreRequerido() };
            yield return new object[] { new CrearCategoriaComando(new string('a', 51)), new ErrorLongitudInvalida("nombre de la categoría", 50) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearCategoria_BadRequest))]
        public void CrearCategoria_Invalida_RetornaBadRequest(CrearCategoriaComando comando, System.Exception excepcionLanzada)
        {
            _categoriaServiceMock.Setup(s => s.Crear(It.IsAny<CrearCategoriaComando>())).Returns(Result<CategoriaDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _categoriasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearCategoria_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearCategoriaComando("Cable");
            _categoriaServiceMock.Setup(s => s.Crear(It.IsAny<CrearCategoriaComando>())).Returns(Result<CategoriaDto>.Conflict());
            var resultado = _categoriasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearCategoria_ServicioError_RetornaError500()
        {
            var comando = new CrearCategoriaComando("Error General");
            _categoriaServiceMock.Setup(s => s.Crear(It.IsAny<CrearCategoriaComando>())).Returns(Result<CategoriaDto>.Error("Error General Servidor"));
            var resultado = _categoriasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarCategoria_Valida_RetornaOk()
        {
            var comando = new ActualizarCategoriaComando(4, "Prueba Actualizada");
            var dto = new CategoriaDto { Id = 4, Nombre = "Prueba Actualizada" };
            _categoriaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.Success(dto));
            var resultado = _categoriasController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarCategoria_BadRequest()
        {
            yield return new object[] { new ActualizarCategoriaComando(0, "Inválido"), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarCategoriaComando(1, ""), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarCategoriaComando(1, new string('a', 51)), new ErrorLongitudInvalida("nombre de la categoría", 50) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarCategoria_BadRequest))]
        public void ActualizarCategoria_Invalida_RetornaBadRequest(ActualizarCategoriaComando comando, System.Exception excepcionLanzada)
        {
            _categoriaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _categoriasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarCategoria_NoEncontrada_RetornaNotFound()
        {
            var comando = new ActualizarCategoriaComando(99, "NoExiste");
            _categoriaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.NotFound());
            var resultado = _categoriasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarCategoria_RegistroExistente_RetornaConflict()
        {
            var comando = new ActualizarCategoriaComando(1, "Cable");
            _categoriaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.Conflict());
            var resultado = _categoriasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ActualizarCategoria_ServicioError_RetornaError500()
        {
            var comando = new ActualizarCategoriaComando(1, "Error General");
            _categoriaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCategoriaComando>())).Returns(Result<CategoriaDto>.Error("Error General Servidor"));
            var resultado = _categoriasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarCategoria_Valida_RetornaNoContent()
        {
            int idValido = 10;
            var dto = new CategoriaDto { Id = idValido };
            _categoriaServiceMock.Setup(s => s.Eliminar(It.Is<EliminarCategoriaComando>(c => c.Id == idValido))).Returns(Result<CategoriaDto>.Success(dto));
            var resultado = _categoriasController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarCategoria_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarCategoria_BadRequest))]
        public void EliminarCategoria_Invalida_RetornaBadRequest(int idCategoria, System.Exception excepcionLanzada)
        {
            _categoriaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCategoriaComando>())).Returns(Result<CategoriaDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _categoriasController.Eliminar(idCategoria);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarCategoria_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _categoriaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCategoriaComando>())).Returns(Result<CategoriaDto>.NotFound());
            var resultado = _categoriasController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarCategoria_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _categoriaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCategoriaComando>())).Returns(Result<CategoriaDto>.Conflict());
            var resultado = _categoriasController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarCategoria_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _categoriaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCategoriaComando>())).Returns(Result<CategoriaDto>.Error("Error General Servidor"));
            var resultado = _categoriasController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
