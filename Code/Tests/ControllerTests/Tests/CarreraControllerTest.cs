using Moq;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Interfaces;

namespace IMT_Reservas.Tests.ControllerTests.Tests
{
    [TestFixture]
    public class CarreraControllerTest : ICarreraControllerTest
    {
        private Mock<ICarreraService>    _carreraServiceMock;
        private CarreraController       _carrerasController;

        [SetUp]
        public void Setup()
        {
            _carreraServiceMock = new Mock<ICarreraService>();
            _carrerasController = new CarreraController(_carreraServiceMock.Object);
        }

        [Test]
        public void GetCarreras_ConDatos_RetornaOk()
        {
            List<CarreraDto> carrerasEsperadas = new List<CarreraDto>
            {
                new CarreraDto { Id = 1, Nombre = "Mecatronica" },
                new CarreraDto { Id = 2, Nombre = "Software" }
            };
            _carreraServiceMock.Setup(s => s.ObtenerTodasCarreras()).Returns(carrerasEsperadas);
            ActionResult<List<CarreraDto>> resultadoAccion = _carrerasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<CarreraDto>>().And.Count.EqualTo(carrerasEsperadas.Count));
        }

        [Test]
        public void GetCarreras_SinDatos_RetornaOkVacia()
        {
            List<CarreraDto> carrerasEsperadas = new List<CarreraDto>();
            _carreraServiceMock.Setup(s => s.ObtenerTodasCarreras()).Returns(carrerasEsperadas);
            ActionResult<List<CarreraDto>> resultadoAccion = _carrerasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<CarreraDto>>().And.Empty);
        }

        [Test]
        public void GetCarreras_ServicioError_RetornaBadRequest()
        {
            _carreraServiceMock.Setup(s => s.ObtenerTodasCarreras()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<CarreraDto>> resultadoAccion = _carrerasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearCarrera_Valido_RetornaCreated()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Psicopedagogía");
            _carreraServiceMock.Setup(s => s.CrearCarrera(comando));
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearCarrera_BadRequest()
        {
            yield return new object[] { new CrearCarreraComando(""), new ErrorNombreRequerido() };
            yield return new object[] { new CrearCarreraComando(new string('a', 257)), new ErrorLongitudInvalida("nombre de la carrera", 256) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearCarrera_BadRequest))]
        public void CrearCarrera_Invalido_RetornaBadRequest(CrearCarreraComando comando, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.CrearCarrera(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearCarrera_NombreExistente_RetornaConflict()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Software");
            _carreraServiceMock.Setup(s => s.CrearCarrera(It.IsAny<CrearCarreraComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
        
        [Test]
        public void CrearCarrera_ServicioError_RetornaError500()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Error General");
            _carreraServiceMock.Setup(s => s.CrearCarrera(It.IsAny<CrearCarreraComando>())).Throws(new Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void ActualizarCarrera_Valido_RetornaOk()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(5, "Ingeniería Civil");
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(comando));
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarCarrera_BadRequest()
        {
            yield return new object[] { new ActualizarCarreraComando(0, "Inválido"), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarCarreraComando(1, ""), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarCarreraComando(1, new string('a', 257)), new ErrorLongitudInvalida("nombre de la carrera", 256) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarCarrera_BadRequest))]
        public void ActualizarCarrera_Invalido_RetornaBadRequest(ActualizarCarreraComando comando, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_NoEncontrada_RetornaNotFound()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(99, "NoExiste"); 
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_NombreExistente_RetornaConflict()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Software");
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_ServicioError_RetornaError500()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Error General");
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarCarrera_Valido_RetornaNoContent()
        {
            int idValido = 25;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _carrerasController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarCarrera_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarCarrera_BadRequest))]
        public void EliminarCarrera_Invalido_RetornaBadRequest(int idCarrera, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idCarrera))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Eliminar(idCarrera);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarCarrera_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _carrerasController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarCarrera_EnUso_RetornaConflict()
        {
            int idEnUso = 23;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _carrerasController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarCarrera_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idErrorGeneral))).Throws(new Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        }
    }
}
