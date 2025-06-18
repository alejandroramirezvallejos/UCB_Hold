using API.Controllers;
using Moq;
using Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class EmpresaMantenimientoControllerTest : IEmpresaMantenimientoControllerTest
    {
        private Mock<EmpresaMantenimientoService>    _empresaServiceMock;
        private Mock<EmpresaMantenimientoRepository> _empresaRepoMock;
        private Mock<ExecuteQuery>                   _queryExecMock;
        private Mock<IConfiguration>                 _configMock;
        private EmpresaMantenimientoController       _empresasController;

        [SetUp]
        public void Setup()
        {
            _configMock         = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock      = new Mock<ExecuteQuery>(_configMock.Object);
            _empresaRepoMock    = new Mock<EmpresaMantenimientoRepository>(_queryExecMock.Object);
            _empresaServiceMock = new Mock<EmpresaMantenimientoService>(_empresaRepoMock.Object);
            _empresasController = new EmpresaMantenimientoController(_empresaServiceMock.Object);
        }

        [Test]
        public void GetEmpresas_ConDatos_RetornaOk()
        {
            List<EmpresaMantenimientoDto> empresasEsperadas = new List<EmpresaMantenimientoDto>
            {
                new EmpresaMantenimientoDto { Id = 1, NombreEmpresa = "JJJ" },
                new EmpresaMantenimientoDto { Id = 2, NombreEmpresa = "PRUEBA" }
            };
            _empresaServiceMock.Setup(s => s.ObtenerTodasEmpresasMantenimiento()).Returns(empresasEsperadas);
            ActionResult<List<EmpresaMantenimientoDto>> resultadoAccion = _empresasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<EmpresaMantenimientoDto>>().And.Count.EqualTo(empresasEsperadas.Count));
        }

        [Test]
        public void GetEmpresas_SinDatos_RetornaOkVacia()
        {
            List<EmpresaMantenimientoDto> empresasEsperadas = new List<EmpresaMantenimientoDto>();
            _empresaServiceMock.Setup(s => s.ObtenerTodasEmpresasMantenimiento()).Returns(empresasEsperadas);
            ActionResult<List<EmpresaMantenimientoDto>> resultadoAccion = _empresasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<EmpresaMantenimientoDto>>().And.Empty);
        }

        [Test]
        public void GetEmpresas_ServicioError_RetornaBadRequest()
        {
            _empresaServiceMock.Setup(s => s.ObtenerTodasEmpresasMantenimiento()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<EmpresaMantenimientoDto>> resultadoAccion = _empresasController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearEmpresa_Valida_RetornaCreated()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Mantenimiento Global", "Carlos", "Ruiz", "55555555", "Av. Principal 456", "987654321");
            _empresaServiceMock.Setup(s => s.CrearEmpresaMantenimiento(comando));
            IActionResult resultadoAccion = _empresasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearEmpresa_BadRequest()
        {
            yield return new object[] { new CrearEmpresaMantenimientoComando("", null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEmpresaMantenimientoComando(new string('a', 256), null, null, null, null, null), new ErrorLongitudInvalida("nombre", 255) };
            yield return new object[] { new CrearEmpresaMantenimientoComando("Empresa Valida", null, null, new string('a', 21), null, null), new ErrorLongitudInvalida("telefono", 20) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearEmpresa_BadRequest))]
        public void CrearEmpresa_Invalida_RetornaBadRequest(CrearEmpresaMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.CrearEmpresaMantenimiento(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _empresasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearEmpresa_RegistroExistente_RetornaConflict()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("PRUEBA", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.CrearEmpresaMantenimiento(It.IsAny<CrearEmpresaMantenimientoComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _empresasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void CrearEmpresa_ServicioError_RetornaError500()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Error General", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.CrearEmpresaMantenimiento(It.IsAny<CrearEmpresaMantenimientoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _empresasController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ActualizarEmpresa_Valida_RetornaOk()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "JJJ Actualizado", "string", "string", "string", "string", "string");
            _empresaServiceMock.Setup(s => s.ActualizarEmpresaMantenimiento(comando));
            IActionResult resultadoAccion = _empresasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarEmpresa_BadRequest()
        {
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(0, "Inválido", null, null, null, null, null), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "", null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, new string('a', 101), null, null, null, null, null), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "Empresa Valida", null, null, new string('a', 21), null, null), new ErrorLongitudInvalida("telefono", 20) };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "Empresa Valida", null, null, null, null, new string('a', 21)), new ErrorLongitudInvalida("nit", 20) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarEmpresa_BadRequest))]
        public void ActualizarEmpresa_Invalida_RetornaBadRequest(ActualizarEmpresaMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.ActualizarEmpresaMantenimiento(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _empresasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarEmpresa_NoEncontrada_RetornaNotFound()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(99, "NoExiste", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.ActualizarEmpresaMantenimiento(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _empresasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarEmpresa_RegistroExistente_RetornaConflict()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(3, "PRUEBA", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.ActualizarEmpresaMantenimiento(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _empresasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarEmpresa_ServicioError_RetornaError500()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "Error General", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.ActualizarEmpresaMantenimiento(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _empresasController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarEmpresa_Valida_RetornaNoContent()
        {
            int idValido = 6;
            _empresaServiceMock.Setup(s => s.EliminarEmpresaMantenimiento(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _empresasController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarEmpresa_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarEmpresa_BadRequest))]
        public void EliminarEmpresa_Invalida_RetornaBadRequest(int idEmpresa, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.EliminarEmpresaMantenimiento(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idEmpresa))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _empresasController.Eliminar(idEmpresa);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarEmpresa_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _empresaServiceMock.Setup(s => s.EliminarEmpresaMantenimiento(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _empresasController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarEmpresa_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _empresaServiceMock.Setup(s => s.EliminarEmpresaMantenimiento(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _empresasController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarEmpresa_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _empresaServiceMock.Setup(s => s.EliminarEmpresaMantenimiento(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _empresasController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
