using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.ResponseDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class PrestamoControllerTests
    {
        private Mock<IPrestamoService> _prestamoServiceMock;
        private PrestamoController _prestamosController;

        [SetUp]
        public void Setup()
        {
            _prestamoServiceMock = new Mock<IPrestamoService>();
            _prestamosController = new PrestamoController(_prestamoServiceMock.Object);
        }

        [Test]
        public void GetPrestamos_ConDatos_RetornaOk()
        {
            var prestamosEsperados = new List<PrestamoDto>
            {
                new PrestamoDto { Id = 5, CarnetUsuario = "12890061" },
                new PrestamoDto { Id = 6, CarnetUsuario = "12890061" }
            };
            _prestamoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<PrestamoDto>>.Success(prestamosEsperados));
            var resultado = _prestamosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(prestamosEsperados.Count));
        }

        [Test]
        public void GetPrestamos_SinDatos_RetornaOkVacia()
        {
            _prestamoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<PrestamoDto>>.Success(new List<PrestamoDto>()));
            var resultado = _prestamosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetPrestamos_ServicioError_RetornaError500()
        {
            _prestamoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<PrestamoDto>>.Error("Error servicio"));
            var resultado = _prestamosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearPrestamo_Valido_RetornaOk()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            var dto = new PrestamoConEquiposDto { IdPrestamo = 1 };
            _prestamoServiceMock.Setup(s => s.Crear(It.IsAny<CrearPrestamoComando>())).Returns(Result<PrestamoConEquiposDto>.Created(dto));
            var resultado = _prestamosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearPrestamo_BadRequest()
        {
            var mockFile = new Mock<IFormFile>();
            yield return new object[] { new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "", mockFile.Object), new ErrorNombreRequerido() };
            yield return new object[] { new CrearPrestamoComando(new int[0], DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", mockFile.Object), new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearPrestamo_BadRequest))]
        public void CrearPrestamo_Invalido_RetornaBadRequest(CrearPrestamoComando comando, System.Exception excepcionLanzada)
        {
            _prestamoServiceMock.Setup(s => s.Crear(It.IsAny<CrearPrestamoComando>())).Returns(Result<PrestamoConEquiposDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _prestamosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearPrestamo_UsuarioNoEncontrado_RetornaNotFound()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "00000", mockFile.Object);
            _prestamoServiceMock.Setup(s => s.Crear(It.IsAny<CrearPrestamoComando>())).Returns(Result<PrestamoConEquiposDto>.NotFound());
            var resultado = _prestamosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void CrearPrestamo_SinEquiposDisponibles_RetornaConflict()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            _prestamoServiceMock.Setup(s => s.Crear(It.IsAny<CrearPrestamoComando>())).Returns(Result<PrestamoConEquiposDto>.Conflict());
            var resultado = _prestamosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarPrestamo_Valido_RetornaOk()
        {
            int idValido = 19;
            var dto = new PrestamoDto { Id = idValido };
            _prestamoServiceMock.Setup(s => s.Eliminar(It.Is<EliminarPrestamoComando>(c => c.Id == idValido))).Returns(Result<PrestamoDto>.Success(dto));
            var resultado = _prestamosController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void EliminarPrestamo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _prestamoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarPrestamoComando>())).Returns(Result<PrestamoDto>.NotFound());
            var resultado = _prestamosController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarPrestamo_EnUso_RetornaConflict()
        {
            int idEnUso = 5;
            _prestamoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarPrestamoComando>())).Returns(Result<PrestamoDto>.Conflict());
            var resultado = _prestamosController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ObtenerPorCarnetYEstado_ConDatos_RetornaOk()
        {
            var carnetUsuario = "12890061";
            var estadoPrestamo = "Activo";
            var prestamosEsperados = new List<PrestamoDto>
            {
                new PrestamoDto { Id = 1, CarnetUsuario = carnetUsuario, EstadoPrestamo = estadoPrestamo },
                new PrestamoDto { Id = 2, CarnetUsuario = carnetUsuario, EstadoPrestamo = estadoPrestamo }
            };
            _prestamoServiceMock.Setup(s => s.ObtenerPrestamosPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo)).Returns(prestamosEsperados);
            var resultadoAccion = _prestamosController.ObtenerPorCarnetYEstado(carnetUsuario, estadoPrestamo);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)resultadoAccion;
            Assert.That(okResult.Value, Is.InstanceOf<List<PrestamoDto>>().And.Count.EqualTo(prestamosEsperados.Count));
        }

        [Test]
        public void ObtenerPorCarnetYEstado_SinDatos_RetornaOkVacia()
        {
            var carnetUsuario = "12890061";
            var estadoPrestamo = "Activo";
            _prestamoServiceMock.Setup(s => s.ObtenerPrestamosPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo)).Returns(new List<PrestamoDto>());
            var resultadoAccion = _prestamosController.ObtenerPorCarnetYEstado(carnetUsuario, estadoPrestamo);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)resultadoAccion;
            Assert.That(okResult.Value, Is.InstanceOf<List<PrestamoDto>>().And.Empty);
        }
    }
}
