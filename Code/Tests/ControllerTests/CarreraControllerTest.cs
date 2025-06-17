using Moq;
using API.Controllers;
using Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class CarreraControllerTest
    {
        private Mock<CarreraService>    _carreraServiceMock;
        private Mock<CarreraRepository> _carreraRepoMock;
        private Mock<ExecuteQuery>      _queryExecMock;
        private Mock<IConfiguration>    _configMock;
        private CarreraController       _carrerasController;

        [SetUp]
        public void Setup()
        {
            _configMock         = new Mock<IConfiguration>();
            _queryExecMock      = new Mock<ExecuteQuery>(_configMock.Object);
            _carreraRepoMock    = new Mock<CarreraRepository>(_queryExecMock.Object);
            _carreraServiceMock = new Mock<CarreraService>(_carreraRepoMock.Object);
            _carrerasController = new CarreraController(_carreraServiceMock.Object);
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
        }

        [Test]
        public void GetCarreras_ConDatos_RetornaOk()
        {
            List<CarreraDto> carrerasEsperadas = new List<CarreraDto>
            {
                new CarreraDto { Id = 1, Nombre = "Ingeniería de Sistemas" },
                new CarreraDto { Id = 2, Nombre = "Derecho" }
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
            CrearCarreraComando comando = new CrearCarreraComando("Nueva Carrera");
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearCarrera_BadRequest()
        {
            yield return new object[] { new CrearCarreraComando(""), new ErrorNombreRequerido("nombre") };
            yield return new object[] { new CrearCarreraComando(new string('a', 101)), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new CrearCarreraComando("ErrorDominio"), new ErrorIdInvalido("ID Inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearCarrera_BadRequest))]
        public void CrearCarrera_Invalido_RetornaBadRequest(CrearCarreraComando comando, System.Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.CrearCarrera(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearCarrera_NombreExistente_RetornaConflict()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Carrera Existente");
            _carreraServiceMock.Setup(s => s.CrearCarrera(It.IsAny<CrearCarreraComando>())).Throws(new ErrorRegistroYaExiste("Existe"));
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
        
        [Test]
        public void CrearCarrera_ServicioError_RetornaError500()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Error General");
            _carreraServiceMock.Setup(s => s.CrearCarrera(It.IsAny<CrearCarreraComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void ActualizarCarrera_Valido_RetornaOk()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Carrera Actualizada");
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarCarrera_BadRequest()
        {
            yield return new object[] { new ActualizarCarreraComando(0, "Inválido"), new ErrorIdInvalido("ID") };
            yield return new object[] { new ActualizarCarreraComando(1, ""), new ErrorNombreRequerido("nombre") };
            yield return new object[] { new ActualizarCarreraComando(1, new string('a', 101)), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new ActualizarCarreraComando(1, "ErrorDominio"), new ErrorRegistroEnUso("EnUso") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarCarrera_BadRequest))]
        public void ActualizarCarrera_Invalido_RetornaBadRequest(ActualizarCarreraComando comando, System.Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_NoEncontrada_RetornaNotFound()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(99, "NoExiste"); 
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new ErrorRegistroNoEncontrado("NoEncontrada"));
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_NombreExistente_RetornaConflict()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Nombre Existente");
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new ErrorRegistroYaExiste("Existe"));
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarCarrera_ServicioError_RetornaError500()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Error General");
            _carreraServiceMock.Setup(s => s.ActualizarCarrera(It.IsAny<ActualizarCarreraComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarCarrera_Valido_RetornaNoContent()
        {
            int idValido = 1;
            IActionResult resultadoAccion = _carrerasController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarCarrera_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("ID") };
            yield return new object[] { 3, new ErrorNombreRequerido("NombreReq") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarCarrera_BadRequest))]
        public void EliminarCarrera_Invalido_RetornaBadRequest(int idCarrera, System.Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idCarrera))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _carrerasController.Eliminar(idCarrera);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarCarrera_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado("NoEncontrada"));
            IActionResult resultadoAccion = _carrerasController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarCarrera_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso("EnUso"));
            IActionResult resultadoAccion = _carrerasController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarCarrera_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _carreraServiceMock.Setup(s => s.EliminarCarrera(It.Is<EliminarCarreraComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _carrerasController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
