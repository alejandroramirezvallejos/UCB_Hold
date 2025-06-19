using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Interfaces;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class GaveteroControllerTest : IGaveteroControllerTest
    {
        private Mock<IGaveteroService>    _gaveteroServiceMock;
        private GaveteroController       _gaveterosController;

        [SetUp]
        public void Setup()
        {
            _gaveteroServiceMock = new Mock<IGaveteroService>();
            _gaveterosController = new GaveteroController(_gaveteroServiceMock.Object);
        }

        [Test]
        public void GetGaveteros_ConDatos_RetornaOk()
        {
            List<GaveteroDto> gaveterosEsperados = new List<GaveteroDto>
            {
                new GaveteroDto { Id = 1, Nombre = "prueba" },
                new GaveteroDto { Id = 2, Nombre = "JJJJ" }
            };
            _gaveteroServiceMock.Setup(s => s.ObtenerTodosGaveteros()).Returns(gaveterosEsperados);
            ActionResult<List<GaveteroDto>> resultadoAccion = _gaveterosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<GaveteroDto>>().And.Count.EqualTo(gaveterosEsperados.Count));
        }

        [Test]
        public void GetGaveteros_SinDatos_RetornaOkVacia()
        {
            List<GaveteroDto> gaveterosEsperados = new List<GaveteroDto>();
            _gaveteroServiceMock.Setup(s => s.ObtenerTodosGaveteros()).Returns(gaveterosEsperados);
            ActionResult<List<GaveteroDto>> resultadoAccion = _gaveterosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<GaveteroDto>>().And.Empty);
        }

        [Test]
        public void GetGaveteros_ServicioError_RetornaBadRequest()
        {
            _gaveteroServiceMock.Setup(s => s.ObtenerTodosGaveteros()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<GaveteroDto>> resultadoAccion = _gaveterosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearGavetero_Valido_RetornaCreated()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-05", "Almacenamiento", "Mueble-C", 70, 50, 30);
            _gaveteroServiceMock.Setup(s => s.CrearGavetero(comando));
            IActionResult resultadoAccion = _gaveterosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearGavetero_BadRequest()
        {
            yield return new object[] { new CrearGaveteroComando("", "Tipo", "Mueble", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", -10, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", null, -10, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", null, null, -10), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearGavetero_BadRequest))]
        public void CrearGavetero_Invalido_RetornaBadRequest(CrearGaveteroComando comando, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.CrearGavetero(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _gaveterosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearGavetero_RegistroExistente_RetornaConflict()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("FER", "MAGIA", "Mueble-D", null, null, null);
            _gaveteroServiceMock.Setup(s => s.CrearGavetero(It.IsAny<CrearGaveteroComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _gaveterosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void CrearGavetero_ServicioError_RetornaError500()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("Error General", "Tipo", "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.CrearGavetero(It.IsAny<CrearGaveteroComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _gaveterosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ActualizarGavetero_Valido_RetornaOk()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(3, "FER Actualizado", "MAGIA v2", "Mueble-E", 1.5, 3.2, 1.2);
            _gaveteroServiceMock.Setup(s => s.ActualizarGavetero(comando));
            IActionResult resultadoAccion = _gaveterosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarGavetero_BadRequest()
        {
            yield return new object[] { new ActualizarGaveteroComando(0, "Inválido", null, null, null, null, null), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarGaveteroComando(1, "", null, "Mueble", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGaveteroComando(1, "Nombre", null, "", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGaveteroComando(1, "Nombre", null, "Mueble", -10, null, null), new ErrorValorNegativo("longitud") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarGavetero_BadRequest))]
        public void ActualizarGavetero_Invalido_RetornaBadRequest(ActualizarGaveteroComando comando, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.ActualizarGavetero(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _gaveterosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarGavetero_NoEncontrado_RetornaNotFound()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(99, "NoExiste", null, "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.ActualizarGavetero(It.IsAny<ActualizarGaveteroComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _gaveterosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarGavetero_ServicioError_RetornaError500()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(1, "Error General", null, "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.ActualizarGavetero(It.IsAny<ActualizarGaveteroComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _gaveterosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarGavetero_Valido_RetornaNoContent()
        {
            int idValido = 1;
            _gaveteroServiceMock.Setup(s => s.EliminarGavetero(It.Is<EliminarGaveteroComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _gaveterosController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarGavetero_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarGavetero_BadRequest))]
        public void EliminarGavetero_Invalido_RetornaBadRequest(int idGavetero, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.EliminarGavetero(It.Is<EliminarGaveteroComando>(c => c.Id == idGavetero))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _gaveterosController.Eliminar(idGavetero);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarGavetero_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _gaveteroServiceMock.Setup(s => s.EliminarGavetero(It.Is<EliminarGaveteroComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _gaveterosController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarGavetero_EnUso_RetornaConflict()
        {
            int idEnUso = 4;
            _gaveteroServiceMock.Setup(s => s.EliminarGavetero(It.Is<EliminarGaveteroComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _gaveterosController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarGavetero_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _gaveteroServiceMock.Setup(s => s.EliminarGavetero(It.Is<EliminarGaveteroComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _gaveterosController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
