using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MuebleControllerTest : IMuebleControllerTest
    {
        private Mock<MuebleService>    _muebleServiceMock;
        private Mock<MuebleRepository> _muebleRepoMock;
        private Mock<ExecuteQuery>     _queryExecMock;
        private Mock<IConfiguration>   _configMock;
        private MuebleController       _mueblesController;

        [SetUp]
        public void Setup()
        {
            _configMock        = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock     = new Mock<ExecuteQuery>(_configMock.Object);
            _muebleRepoMock    = new Mock<MuebleRepository>(_queryExecMock.Object);
            _muebleServiceMock = new Mock<MuebleService>(_muebleRepoMock.Object);
            _mueblesController = new MuebleController(_muebleServiceMock.Object);
        }

        [Test]
        public void GetMuebles_ConDatos_RetornaOk()
        {
            List<MuebleDto> mueblesEsperados = new List<MuebleDto>
            {
                new MuebleDto { Id = 3, Nombre = "FERRR" },
                new MuebleDto { Id = 4, Nombre = "ferprueba" }
            };
            _muebleServiceMock.Setup(s => s.ObtenerTodosMuebles()).Returns(mueblesEsperados);
            ActionResult<List<MuebleDto>> resultadoAccion = _mueblesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<MuebleDto>>().And.Count.EqualTo(mueblesEsperados.Count));
        }

        [Test]
        public void GetMuebles_SinDatos_RetornaOkVacia()
        {
            List<MuebleDto> mueblesEsperados = new List<MuebleDto>();
            _muebleServiceMock.Setup(s => s.ObtenerTodosMuebles()).Returns(mueblesEsperados);
            ActionResult<List<MuebleDto>> resultadoAccion = _mueblesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<MuebleDto>>().And.Empty);
        }

        [Test]
        public void GetMuebles_ServicioError_RetornaBadRequest()
        {
            _muebleServiceMock.Setup(s => s.ObtenerTodosMuebles()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<MuebleDto>> resultadoAccion = _mueblesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearMueble_Valido_RetornaCreated()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Armario Metálico", "Almacenamiento", 250.00, "Depósito 2", 180.0, 90.0, 45.0);
            _muebleServiceMock.Setup(s => s.CrearMueble(comando));
            IActionResult resultadoAccion = _mueblesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearMueble_BadRequest()
        {
            yield return new object[] { new CrearMuebleComando("", "Tipo", null, "Ubicacion", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", -10, "Ubicacion", null, null, null), new ErrorValorNegativo("costo") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", 0, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", null, 0, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", null, null, 0), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearMueble_BadRequest))]
        public void CrearMueble_Invalido_RetornaBadRequest(CrearMuebleComando comando, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.CrearMueble(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _mueblesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearMueble_RegistroExistente_RetornaConflict()
        {
            CrearMuebleComando comando = new CrearMuebleComando("ferprueba", "prueba", null, "x", null, null, null);
            _muebleServiceMock.Setup(s => s.CrearMueble(It.IsAny<CrearMuebleComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _mueblesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void CrearMueble_ServicioError_RetornaError500()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Error General", "Tipo", null, "Ubicacion", null, null, null);
            _muebleServiceMock.Setup(s => s.CrearMueble(It.IsAny<CrearMuebleComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _mueblesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ActualizarMueble_Valido_RetornaOk()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(4, "ferprueba-actualizado", "prueba-v2", 120.00, "y", 1.5, 4.2, 0.6);
            _muebleServiceMock.Setup(s => s.ActualizarMueble(comando));
            IActionResult resultadoAccion = _mueblesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarMueble_BadRequest()
        {
            yield return new object[] { new ActualizarMuebleComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarMuebleComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, -10, null, null, null, null), new ErrorValorNegativo("costo") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, 0, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, null, 0, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, null, null, 0), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarMueble_BadRequest))]
        public void ActualizarMueble_Invalido_RetornaBadRequest(ActualizarMuebleComando comando, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.ActualizarMueble(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _mueblesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarMueble_NoEncontrado_RetornaNotFound()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(99, "NoExiste", null, null, null, null, null, null);
            _muebleServiceMock.Setup(s => s.ActualizarMueble(It.IsAny<ActualizarMuebleComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _mueblesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarMueble_ServicioError_RetornaError500()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(1, "Error General", null, null, null, null, null, null);
            _muebleServiceMock.Setup(s => s.ActualizarMueble(It.IsAny<ActualizarMuebleComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _mueblesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarMueble_Valido_RetornaNoContent()
        {
            int idValido = 3;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _mueblesController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarMueble_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarMueble_BadRequest))]
        public void EliminarMueble_Invalido_RetornaBadRequest(int idMueble, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idMueble))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _mueblesController.Eliminar(idMueble);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarMueble_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _mueblesController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarMueble_EnUso_RetornaConflict()
        {
            int idEnUso = 5;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _mueblesController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarMueble_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _mueblesController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
