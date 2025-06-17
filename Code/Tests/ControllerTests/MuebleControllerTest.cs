using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MuebleControllerTest
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
            _queryExecMock     = new Mock<ExecuteQuery>(_configMock.Object);
            _muebleRepoMock    = new Mock<MuebleRepository>(_queryExecMock.Object);
            _muebleServiceMock = new Mock<MuebleService>(_muebleRepoMock.Object);
            _mueblesController = new MuebleController(_muebleServiceMock.Object);
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
        }

        [Test]
        public void GetMuebles_ConDatos_RetornaOk()
        {
            List<MuebleDto> mueblesEsperados = new List<MuebleDto>
            {
                new MuebleDto { Id = 1, Nombre = "Escritorio" },
                new MuebleDto { Id = 2, Nombre = "Silla" }
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
            CrearMuebleComando comando = new CrearMuebleComando("Mesa", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);
            _muebleServiceMock.Setup(s => s.CrearMueble(comando));
            IActionResult resultadoAccion = _mueblesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearMueble_BadRequest()
        {
            yield return new object[] { new CrearMuebleComando("", "Tipo", null, "Ubicacion", null, null, null), new ErrorNombreRequerido("nombre del mueble") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", -10.0, null, null), new ErrorValorNegativo("longitud") };
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
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", null, "Sala 1", null, null, null);
            _muebleServiceMock.Setup(s => s.CrearMueble(It.IsAny<CrearMuebleComando>())).Throws(new ErrorRegistroYaExiste("Escritorio"));
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
            ActualizarMuebleComando comando = new ActualizarMuebleComando(1, "Mesa de Reuniones", "Oficina", 250.00, "Sala 2", 200.0, 100.0, 75.0);
            _muebleServiceMock.Setup(s => s.ActualizarMueble(comando));
            IActionResult resultadoAccion = _mueblesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarMueble_BadRequest()
        {
            yield return new object[] { new ActualizarMuebleComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido("mueble") };
            yield return new object[] { new ActualizarMuebleComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido("nombre del mueble") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, -10.0, null, null), new ErrorValorNegativo("longitud") };
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
            _muebleServiceMock.Setup(s => s.ActualizarMueble(It.IsAny<ActualizarMuebleComando>())).Throws(new ErrorRegistroNoEncontrado("99"));
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
            int idValido = 1;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _mueblesController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void EliminarMueble_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado("99"));
            IActionResult resultadoAccion = _mueblesController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarMueble_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _muebleServiceMock.Setup(s => s.EliminarMueble(It.Is<EliminarMuebleComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso("El mueble tiene gaveteros asociados"));
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
