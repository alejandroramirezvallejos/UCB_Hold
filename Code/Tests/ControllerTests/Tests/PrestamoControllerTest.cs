using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class PrestamoControllerTests : IPrestamoControllerTest
    {
        private Mock<PrestamoService>    _prestamoServiceMock;
        private Mock<PrestamoRepository> _prestamoRepoMock;
        private Mock<ExecuteQuery>       _queryExecMock;
        private Mock<IConfiguration>     _configMock;
        private PrestamoController       _prestamosController;

        [SetUp]
        public void Setup()
        {
            _configMock          = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock       = new Mock<ExecuteQuery>(_configMock.Object);
            _prestamoRepoMock    = new Mock<PrestamoRepository>(_queryExecMock.Object);
            _prestamoServiceMock = new Mock<PrestamoService>(_prestamoRepoMock.Object);
            _prestamosController = new PrestamoController(_prestamoServiceMock.Object);
        }

        [Test]
        public void GetPrestamos_ConDatos_RetornaOk()
        {
            List<PrestamoDto> prestamosEsperados = new List<PrestamoDto>
            {
                new PrestamoDto { Id = 5, CarnetUsuario = "12890061" },
                new PrestamoDto { Id = 6, CarnetUsuario = "12890061" }
            };
            _prestamoServiceMock.Setup(s => s.ObtenerTodosPrestamos()).Returns(prestamosEsperados);
            IActionResult resultadoAccion = _prestamosController.ObtenerPrestamos();
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<PrestamoDto>>().And.Count.EqualTo(prestamosEsperados.Count));
        }

        [Test]
        public void GetPrestamos_SinDatos_RetornaOkVacia()
        {
            List<PrestamoDto> prestamosEsperados = new List<PrestamoDto>();
            _prestamoServiceMock.Setup(s => s.ObtenerTodosPrestamos()).Returns(prestamosEsperados);
            IActionResult resultadoAccion = _prestamosController.ObtenerPrestamos();
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<PrestamoDto>>().And.Empty);
        }

        [Test]
        public void GetPrestamos_ServicioError_RetornaError500()
        {
            _prestamoServiceMock.Setup(s => s.ObtenerTodosPrestamos()).Throws(new System.Exception("Error servicio"));
            IActionResult resultadoAccion = _prestamosController.ObtenerPrestamos();
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void CrearPrestamo_Valido_RetornaOk()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", null);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(comando));
            IActionResult resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearPrestamo_BadRequest()
        {
            yield return new object[] { new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "", null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearPrestamoComando(new int[0], DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", null), new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearPrestamo_BadRequest))]
        public void CrearPrestamo_Invalido_RetornaBadRequest(CrearPrestamoComando comando, System.Exception excepcionLanzada)
        {
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearPrestamo_UsuarioNoEncontrado_RetornaNotFound()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "00000", null);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(It.IsAny<CrearPrestamoComando>())).Throws(new ErrorCarnetUsuarioNoEncontrado());
            IActionResult resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void CrearPrestamo_SinEquiposDisponibles_RetornaConflict()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", null);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(It.IsAny<CrearPrestamoComando>())).Throws(new ErrorNoEquiposDisponibles());
            IActionResult resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarPrestamo_Valido_RetornaOk()
        {
            int idValido = 19;
            _prestamoServiceMock.Setup(s => s.EliminarPrestamo(It.Is<EliminarPrestamoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _prestamosController.EliminarPrestamo(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void EliminarPrestamo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _prestamoServiceMock.Setup(s => s.EliminarPrestamo(It.Is<EliminarPrestamoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _prestamosController.EliminarPrestamo(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarPrestamo_EnUso_RetornaConflict()
        {
            int idEnUso = 5;
            _prestamoServiceMock.Setup(s => s.EliminarPrestamo(It.Is<EliminarPrestamoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _prestamosController.EliminarPrestamo(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
    }
}
