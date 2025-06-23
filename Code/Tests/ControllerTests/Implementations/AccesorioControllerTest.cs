using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class AccesorioControllerTest : IAccesorioControllerTest
    {
        private Mock<IAccesorioService> _accesorioServiceMock;
        private AccesorioController _accesoriosController;

        [SetUp]
        public void Setup()
        {
            _accesorioServiceMock = new Mock<IAccesorioService>();
            _accesoriosController = new AccesorioController(_accesorioServiceMock.Object);
        }

        [Test]
        public void GetAccesorios_ConDatos_RetornaOk()
        {
            List<AccesorioDto> accesoriosEsperados = new List<AccesorioDto>
            {
                new AccesorioDto { Id = 2, Nombre = "cable usb", Modelo = "dasd" },
                new AccesorioDto { Id = 3, Nombre = "string", Modelo = "string" }
            };
            _accesorioServiceMock.Setup(s => s.ObtenerTodosAccesorios()).Returns(accesoriosEsperados);
            IActionResult resultadoAccion = _accesoriosController.ObtenerTodos();
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<AccesorioDto>>().And.Count.EqualTo(accesoriosEsperados.Count));
        }

        [Test]
        public void GetAccesorios_SinDatos_RetornaOkVacia()
        {
            List<AccesorioDto> accesoriosEsperados = new List<AccesorioDto>();
            _accesorioServiceMock.Setup(s => s.ObtenerTodosAccesorios()).Returns(accesoriosEsperados);
            IActionResult resultadoAccion = _accesoriosController.ObtenerTodos();
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<AccesorioDto>>().And.Empty);
        }

        [Test]
        public void GetAccesorios_ServicioError_RetornaBadRequest()
        {
            _accesorioServiceMock.Setup(s => s.ObtenerTodosAccesorios()).Throws(new System.Exception("Error servicio"));
            IActionResult resultadoAccion = _accesoriosController.ObtenerTodos();
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearAccesorio_Valido_RetornaCreated()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Nuevo Cable", "NC-01", "Electrónico", 777, "Un cable nuevo", 20.00, "http://example.com/nc01.pdf");
            _accesorioServiceMock.Setup(s => s.CrearAccesorio(comando));
            IActionResult resultadoAccion = _accesoriosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearAccesorio_BadRequest()
        {
            yield return new object[] { new CrearAccesorioComando("", "ModeloX", "TipoY", 1004, "desc", 10.0, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearAccesorioComando("NombreValido", null, "TipoY", 1004, "desc", 10.0, null), new ErrorModeloRequerido() };
            yield return new object[] { new CrearAccesorioComando(new string('a', 257), "ModeloX", "TipoY", 1005, "desc", 10.0, null), new ErrorLongitudInvalida("nombre del accesorio", 256) };
            yield return new object[] { new CrearAccesorioComando("NombreValido", "ModeloX", "TipoY", 0, "desc", 10.0, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new CrearAccesorioComando("NombreValido", "ModeloX", "TipoY", 1, "desc", -1.0, null), new ErrorValorNegativo("precio") };
            yield return new object[] { new CrearAccesorioComando("ReferenciaInv", "ModeloR", "TipoB", 1007, "desc", 10.0, null), new ErrorReferenciaInvalida("Referencia Inválida") }; 
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearAccesorio_BadRequest))]
        public void CrearAccesorio_Invalido_RetornaBadRequest(CrearAccesorioComando comando, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.CrearAccesorio(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _accesoriosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearAccesorio_RegistroExistente_RetornaConflict()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("cable usb", "dasd", "Electrónico", 123, "desc", 15.99, null);
            _accesorioServiceMock.Setup(s => s.CrearAccesorio(It.IsAny<CrearAccesorioComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _accesoriosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
        
        [Test]
        public void CrearAccesorio_ServicioError_RetornaError500()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Error General", "ModeloErr", "TipoErr", 9999, "desc", 0, null);
            _accesorioServiceMock.Setup(s => s.CrearAccesorio(It.IsAny<CrearAccesorioComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _accesoriosController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void ActualizarAccesorio_Valido_RetornaOk()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(2, "cable usb-c", "dasd-2", "Electrónico", 123, "cable usb actualizado", 19.99, "https://datasheet.example.com/c123-v2.pdf");
            _accesorioServiceMock.Setup(s => s.ActualizarAccesorio(comando));
            IActionResult resultadoAccion = _accesoriosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarAccesorio_BadRequest()
        {
            yield return new object[] { new ActualizarAccesorioComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarAccesorioComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarAccesorioComando(1, new string('a', 101), null, null, null, null, null, null), new ErrorLongitudInvalida("nombre del accesorio", 100) };
            yield return new object[] { new ActualizarAccesorioComando(1, "Nombre Valido", null, null, 0, null, null, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new ActualizarAccesorioComando(1, "Nombre Valido", null, null, 1, null, -1, null), new ErrorValorNegativo("precio") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarAccesorio_BadRequest))]
        public void ActualizarAccesorio_Invalido_RetornaBadRequest(ActualizarAccesorioComando comando, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.ActualizarAccesorio(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _accesoriosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarAccesorio_NoEncontrado_RetornaNotFound()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(99, "NoExiste", null, null, null, null, null, null); 
            _accesorioServiceMock.Setup(s => s.ActualizarAccesorio(It.IsAny<ActualizarAccesorioComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _accesoriosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarAccesorio_RegistroExistente_RetornaConflict()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(1, "cable usb", null, null, 1002, null, null, null);
            _accesorioServiceMock.Setup(s => s.ActualizarAccesorio(It.IsAny<ActualizarAccesorioComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _accesoriosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarAccesorio_ServicioError_RetornaError500()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(1, "Error General", null, null, null, null, null, null);
            _accesorioServiceMock.Setup(s => s.ActualizarAccesorio(It.IsAny<ActualizarAccesorioComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _accesoriosController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarAccesorio_Valido_RetornaNoContent()
        {
            int idValido = 3;
            _accesorioServiceMock.Setup(s => s.EliminarAccesorio(It.Is<EliminarAccesorioComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _accesoriosController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarAccesorio_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarAccesorio_BadRequest))]
        public void EliminarAccesorio_Invalido_RetornaBadRequest(int idAccesorio, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.EliminarAccesorio(It.Is<EliminarAccesorioComando>(c => c.Id == idAccesorio))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _accesoriosController.Eliminar(idAccesorio);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarAccesorio_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _accesorioServiceMock.Setup(s => s.EliminarAccesorio(It.Is<EliminarAccesorioComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _accesoriosController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarAccesorio_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _accesorioServiceMock.Setup(s => s.EliminarAccesorio(It.Is<EliminarAccesorioComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _accesoriosController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarAccesorio_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _accesorioServiceMock.Setup(s => s.EliminarAccesorio(It.Is<EliminarAccesorioComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _accesoriosController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
