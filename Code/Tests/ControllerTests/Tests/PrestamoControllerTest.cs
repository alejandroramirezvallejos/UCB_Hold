using API.Controllers;
using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class PrestamoControllerTests : IPrestamoControllerTest
    {
        private Mock<IPrestamoService>    _prestamoServiceMock;
        private PrestamoController       _prestamosController;

        [SetUp]
        public void Setup()
        {
            _prestamoServiceMock = new Mock<IPrestamoService>();
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
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(comando));
            
            var resultadoAccion = _prestamosController.CrearPrestamo(comando);
            
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearPrestamo_BadRequest()
        {
            var mockFile = new Mock<IFormFile>();
            yield return new object[] { new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "", mockFile.Object), new ErrorNombreRequerido() };
            yield return new object[] { new CrearPrestamoComando(new int[0], DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", mockFile.Object), new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearPrestamo_BadRequest))]
        public void CrearPrestamo_Invalido_RetornaBadRequest(CrearPrestamoComando comando, System.Exception excepcionLanzada)
        {
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(comando)).Throws(excepcionLanzada);
            var resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearPrestamo_UsuarioNoEncontrado_RetornaNotFound()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "00000", mockFile.Object);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(It.IsAny<CrearPrestamoComando>())).Throws(new ErrorCarnetUsuarioNoEncontrado());
            var resultadoAccion = _prestamosController.CrearPrestamo(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void CrearPrestamo_SinEquiposDisponibles_RetornaConflict()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            _prestamoServiceMock.Setup(s => s.CrearPrestamo(It.IsAny<CrearPrestamoComando>())).Throws(new ErrorNoEquiposDisponibles());
            var resultadoAccion = _prestamosController.CrearPrestamo(comando);
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

        [Test]
        public void AceptarPrestamo_Valido_RetornaOk()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var comando = new AceptarPrestamoComando { PrestamoId = 1, Contrato = mockFile.Object };
            _prestamoServiceMock.Setup(s => s.AceptarPrestamo(comando));

            // Act
            var resultadoAccion = _prestamosController.AceptarPrestamo(comando);

            // Assert
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void AceptarPrestamo_ArgumentoInvalido_RetornaBadRequest()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var comando = new AceptarPrestamoComando { PrestamoId = 1, Contrato = mockFile.Object };
            _prestamoServiceMock.Setup(s => s.AceptarPrestamo(comando)).Throws(new ArgumentException("Argumento inválido"));

            // Act
            var resultadoAccion = _prestamosController.AceptarPrestamo(comando);

            // Assert
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AceptarPrestamo_ErrorServidor_RetornaError500()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var comando = new AceptarPrestamoComando { PrestamoId = 1, Contrato = mockFile.Object };
            _prestamoServiceMock.Setup(s => s.AceptarPrestamo(comando)).Throws(new Exception("Error de servidor"));

            // Act
            var resultadoAccion = _prestamosController.AceptarPrestamo(comando);

            // Assert
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
