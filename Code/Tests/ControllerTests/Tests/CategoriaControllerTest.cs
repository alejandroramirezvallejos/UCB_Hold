using API.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class CategoriaControllerTest : ICategoriaControllerTest
    {
        private Mock<CategoriaService>    _categoriaServiceMock;
        private Mock<CategoriaRepository> _categoriaRepoMock;
        private Mock<ExecuteQuery>        _queryExecMock;
        private Mock<IConfiguration>      _configMock;
        private CategoriaController       _categoriasController;

        [SetUp]
        public void Setup()
        {
            _configMock           = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock        = new Mock<ExecuteQuery>(_configMock.Object);
            _categoriaRepoMock    = new Mock<CategoriaRepository>(_queryExecMock.Object);
            _categoriaServiceMock = new Mock<CategoriaService>(_categoriaRepoMock.Object);
            _categoriasController = new CategoriaController(_categoriaServiceMock.Object);
        }

        [Test]
        public void GetCategorias_ConDatos_RetornaOk()
        {
            List<CategoriaDto> categoriasEsperadas = new List<CategoriaDto>
            {
                new CategoriaDto { Id = 1, Nombre = "Impresora" },
                new CategoriaDto { Id = 3, Nombre = "Cable" }
            };
            _categoriaServiceMock.Setup(s => s.ObtenerTodasCategorias()).Returns(categoriasEsperadas);
            ActionResult<List<CategoriaDto>> resultadoAccion = _categoriasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<CategoriaDto>>().And.Count.EqualTo(categoriasEsperadas.Count));
        }

        [Test]
        public void GetCategorias_SinDatos_RetornaOkVacia()
        {
            List<CategoriaDto> categoriasEsperadas = new List<CategoriaDto>();
            _categoriaServiceMock.Setup(s => s.ObtenerTodasCategorias()).Returns(categoriasEsperadas);
            ActionResult<List<CategoriaDto>> resultadoAccion = _categoriasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<CategoriaDto>>().And.Empty);
        }

        [Test]
        public void GetCategorias_ServicioError_RetornaBadRequest()
        {
            _categoriaServiceMock.Setup(s => s.ObtenerTodasCategorias()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<CategoriaDto>> resultadoAccion = _categoriasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearCategoria_Valida_RetornaCreated()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("Adaptadores"); 
            _categoriaServiceMock.Setup(s => s.CrearCategoria(comando));
            IActionResult resultadoAccion = _categoriasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
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
            _categoriaServiceMock.Setup(s => s.CrearCategoria(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _categoriasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearCategoria_RegistroExistente_RetornaConflict()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("Cable");
            _categoriaServiceMock.Setup(s => s.CrearCategoria(It.IsAny<CrearCategoriaComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _categoriasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
        
        [Test]
        public void CrearCategoria_ServicioError_RetornaError500()
        {
            CrearCategoriaComando comando = new CrearCategoriaComando("Error General");
            _categoriaServiceMock.Setup(s => s.CrearCategoria(It.IsAny<CrearCategoriaComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _categoriasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void ActualizarCategoria_Valida_RetornaOk()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(4, "Prueba Actualizada");
            _categoriaServiceMock.Setup(s => s.ActualizarCategoria(comando)); 
            IActionResult resultadoAccion = _categoriasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarCategoria_BadRequest()
        {
            yield return new object[] { new ActualizarCategoriaComando(0, "Inválido"), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarCategoriaComando(1, ""), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarCategoriaComando(1, new string('a', 51)), new ErrorLongitudInvalida("nombre de la categoría", 50) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarCategoria_BadRequest))]
        public void ActualizarCategoria_Invalida_RetornaBadRequest(ActualizarCategoriaComando comando, System.Exception excepcionLanzada)
        {
            _categoriaServiceMock.Setup(s => s.ActualizarCategoria(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _categoriasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarCategoria_NoEncontrada_RetornaNotFound()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(99, "NoExiste"); 
            _categoriaServiceMock.Setup(s => s.ActualizarCategoria(It.IsAny<ActualizarCategoriaComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _categoriasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarCategoria_RegistroExistente_RetornaConflict()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(1, "Cable");
            _categoriaServiceMock.Setup(s => s.ActualizarCategoria(It.IsAny<ActualizarCategoriaComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _categoriasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarCategoria_ServicioError_RetornaError500()
        {
            ActualizarCategoriaComando comando = new ActualizarCategoriaComando(1, "Error General");
            _categoriaServiceMock.Setup(s => s.ActualizarCategoria(It.IsAny<ActualizarCategoriaComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _categoriasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarCategoria_Valida_RetornaNoContent()
        {
            int idValido = 10;
            _categoriaServiceMock.Setup(s => s.EliminarCategoria(It.Is<EliminarCategoriaComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _categoriasController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarCategoria_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarCategoria_BadRequest))]
        public void EliminarCategoria_Invalida_RetornaBadRequest(int idCategoria, System.Exception excepcionLanzada)
        {
            _categoriaServiceMock.Setup(s => s.EliminarCategoria(It.Is<EliminarCategoriaComando>(c => c.Id == idCategoria))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _categoriasController.Eliminar(idCategoria);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarCategoria_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _categoriaServiceMock.Setup(s => s.EliminarCategoria(It.Is<EliminarCategoriaComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _categoriasController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarCategoria_EnUso_RetornaConflict() { 
            int idEnUso = 2;
            _categoriaServiceMock.Setup(s => s.EliminarCategoria(It.Is<EliminarCategoriaComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _categoriasController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarCategoria_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _categoriaServiceMock.Setup(s => s.EliminarCategoria(It.Is<EliminarCategoriaComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _categoriasController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
