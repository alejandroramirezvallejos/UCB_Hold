using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MantenimientoControllerTest : IMantenimientoControllerTest
    {
        private Mock<MantenimientoService>    _mantenimientoServiceMock;
        private Mock<MantenimientoRepository> _mantenimientoRepoMock;
        private Mock<ExecuteQuery>            _queryExecMock;
        private Mock<IConfiguration>          _configMock;
        private MantenimientoController       _mantenimientosController;

        [SetUp]
        public void Setup()
        {
            _configMock               = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock            = new Mock<ExecuteQuery>(_configMock.Object);
            _mantenimientoRepoMock    = new Mock<MantenimientoRepository>(_queryExecMock.Object);
            _mantenimientoServiceMock = new Mock<MantenimientoService>(_mantenimientoRepoMock.Object);
            _mantenimientosController = new MantenimientoController(_mantenimientoServiceMock.Object);
        }

        [Test]
        public void GetMantenimientos_ConDatos_RetornaOk()
        {
            List<MantenimientoDto> mantenimientosEsperados = new List<MantenimientoDto>
            {
                new MantenimientoDto { Id = 1, NombreEmpresaMantenimiento = "Empresa Ficticia 1" },
                new MantenimientoDto { Id = 2, NombreEmpresaMantenimiento = "Empresa Ficticia 1" }
            };
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodosMantenimientos()).Returns(mantenimientosEsperados);
            ActionResult<List<MantenimientoDto>> resultadoAccion = _mantenimientosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<MantenimientoDto>>().And.Count.EqualTo(mantenimientosEsperados.Count));
        }

        [Test]
        public void GetMantenimientos_SinDatos_RetornaOkVacia()
        {
            List<MantenimientoDto> mantenimientosEsperados = new List<MantenimientoDto>();
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodosMantenimientos()).Returns(mantenimientosEsperados);
            ActionResult<List<MantenimientoDto>> resultadoAccion = _mantenimientosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<MantenimientoDto>>().And.Empty);
        }

        [Test]
        public void GetMantenimientos_ServicioError_RetornaBadRequest()
        {
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodosMantenimientos()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<MantenimientoDto>> resultadoAccion = _mantenimientosController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearMantenimiento_Valido_RetornaCreated()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 10), "Empresa Nueva", 300.00, "Mantenimiento de servidor", new int[] { 8 }, new string[] { "Preventivo" }, new string[] { "Servidor Rack" });
            _mantenimientoServiceMock.Setup(s => s.CrearMantenimiento(comando));
            IActionResult resultadoAccion = _mantenimientosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]>
        FuenteCasos_CrearMantenimiento_BadRequest()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorNombreRequerido() };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(-1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorFechaInvalida() };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", null, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, null, new string[] { "Equipo" }), new ErrorReferenciaInvalida("tipos de mantenimiento") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("tipos de mantenimiento") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1, 2 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos y tipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Desc1", "Desc2" }), new ErrorReferenciaInvalida("descripciones") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 0 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorIdInvalido() };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", -100, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorValorNegativo("costo") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearMantenimiento_BadRequest))]
        public void CrearMantenimiento_Invalido_RetornaBadRequest(CrearMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _mantenimientoServiceMock.Setup(s => s.CrearMantenimiento(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _mantenimientosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearMantenimiento_ServicioError_RetornaError500()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Error General", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" });
            _mantenimientoServiceMock.Setup(s => s.CrearMantenimiento(It.IsAny<CrearMantenimientoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _mantenimientosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarMantenimiento_Valido_RetornaNoContent()
        {
            int idValido = 7;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void EliminarMantenimiento_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarMantenimiento_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarMantenimiento_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        private static IEnumerable<object[]> FuenteCasos_EliminarMantenimiento_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarMantenimiento_BadRequest))]
        public void EliminarMantenimiento_Invalido_RetornaBadRequest(int idMantenimiento, System.Exception excepcionLanzada)
        {
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idMantenimiento))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idMantenimiento);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
