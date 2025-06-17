using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MantenimientoControllerTest
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
            _queryExecMock            = new Mock<ExecuteQuery>(_configMock.Object);
            _mantenimientoRepoMock    = new Mock<MantenimientoRepository>(_queryExecMock.Object);
            _mantenimientoServiceMock = new Mock<MantenimientoService>(_mantenimientoRepoMock.Object);
            _mantenimientosController = new MantenimientoController(_mantenimientoServiceMock.Object);
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
        }

        [Test]
        public void GetMantenimientos_ConDatos_RetornaOk()
        {
            List<MantenimientoDto> mantenimientosEsperados = new List<MantenimientoDto>
            {
                new MantenimientoDto { Id = 1, NombreEmpresaMantenimiento = "Empresa A" },
                new MantenimientoDto { Id = 2, NombreEmpresaMantenimiento = "Empresa B" }
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
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "Equipo 1" });
            _mantenimientoServiceMock.Setup(s => s.CrearMantenimiento(comando));
            IActionResult resultadoAccion = _mantenimientosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearMantenimiento_BadRequest()
        {
            yield return new object[] { new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorNombreRequerido("nombre de la empresa de mantenimiento") };
            yield return new object[] { new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("La fecha final debe ser posterior a la fecha de inicio") };
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
            int idValido = 1;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void EliminarMantenimiento_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado("99"));
            IActionResult resultadoAccion = _mantenimientosController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarMantenimiento_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _mantenimientoServiceMock.Setup(s => s.EliminarMantenimiento(It.Is<EliminarMantenimientoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso("El mantenimiento ya fue procesado"));
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
    }
}
