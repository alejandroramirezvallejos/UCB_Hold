using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class EquipoControllerTest : IEquipoControllerTest
    {
        private Mock<EquipoService>    _equipoServiceMock;
        private Mock<EquipoRepository> _equipoRepoMock;
        private Mock<ExecuteQuery>     _queryExecMock;
        private Mock<IConfiguration>   _configMock;
        private EquipoController       _equiposController;

        [SetUp]
        public void Setup()
        {
            _configMock        = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
            _queryExecMock     = new Mock<ExecuteQuery>(_configMock.Object);
            _equipoRepoMock    = new Mock<EquipoRepository>(_queryExecMock.Object);
            _equipoServiceMock = new Mock<EquipoService>(_equipoRepoMock.Object);
            _equiposController = new EquipoController(_equipoServiceMock.Object);
        }

        [Test]
        public void GetEquipos_ConDatos_RetornaOk()
        {
            List<EquipoDto> equiposEsperados = new List<EquipoDto>
            {
                new EquipoDto { Id = 2, NombreGrupoEquipo = "Impresora" },
                new EquipoDto { Id = 4, NombreGrupoEquipo = "Fuente de alimentación DC" }
            };
            _equipoServiceMock.Setup(s => s.ObtenerTodosEquipos()).Returns(equiposEsperados);
            ActionResult<List<EquipoDto>> resultadoAccion = _equiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<EquipoDto>>().And.Count.EqualTo(equiposEsperados.Count));
        }

        [Test]
        public void GetEquipos_SinDatos_RetornaOkVacia()
        {
            List<EquipoDto> equiposEsperados = new List<EquipoDto>();
            _equipoServiceMock.Setup(s => s.ObtenerTodosEquipos()).Returns(equiposEsperados);
            ActionResult<List<EquipoDto>> resultadoAccion = _equiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<EquipoDto>>().And.Empty);
        }

        [Test]
        public void GetEquipos_ServicioError_RetornaBadRequest()
        {
            _equipoServiceMock.Setup(s => s.ObtenerTodosEquipos()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<EquipoDto>> resultadoAccion = _equiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearEquipo_Valido_RetornaCreated()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
            _equipoServiceMock.Setup(s => s.CrearEquipo(comando));
            IActionResult resultadoAccion = _equiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearEquipo_BadRequest()
        {
            yield return new object[] { new CrearEquipoComando("", "Modelo", "Marca", null, null, null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "", "Marca", null, null, null, null, null, null, null, null), new ErrorModeloRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "", null, null, null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "Marca", null, null, null, null, null, -100, null, null), new ErrorValorNegativo("costo de referencia") };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "Marca", null, null, null, null, null, null, 0, null), new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearEquipo_BadRequest))]
        public void CrearEquipo_Invalido_RetornaBadRequest(CrearEquipoComando comando, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.CrearEquipo(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _equiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearEquipo_RegistroExistente_RetornaConflict()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Impresora", "HP", "LaserJet", "JJJJJJ", null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.CrearEquipo(It.IsAny<CrearEquipoComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _equiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void CrearEquipo_ServicioError_RetornaError500()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Error General", "ModeloErr", "MarcaErr", null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.CrearEquipo(It.IsAny<CrearEquipoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _equiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ActualizarEquipo_Valido_RetornaOk()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            _equipoServiceMock.Setup(s => s.ActualizarEquipo(comando));
            IActionResult resultadoAccion = _equiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarEquipo_BadRequest()
        {
            yield return new object[] { new ActualizarEquipoComando(0, null, null, null, null, null, null, null, null, null, null, null, null), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarEquipoComando(1, null, null, null, null, null, null, null, null, -100, null, null, null), new ErrorValorNegativo("costo de referencia") };
            yield return new object[] { new ActualizarEquipoComando(1, null, null, null, null, null, null, null, null, null, 0, null, null), new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarEquipo_BadRequest))]
        public void ActualizarEquipo_Invalido_RetornaBadRequest(ActualizarEquipoComando comando, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.ActualizarEquipo(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _equiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarEquipo_NoEncontrado_RetornaNotFound()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(99, "NoExiste", null, null, null, null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.ActualizarEquipo(It.IsAny<ActualizarEquipoComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _equiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarEquipo_ServicioError_RetornaError500()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(1, "Error General", null, null, null, null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.ActualizarEquipo(It.IsAny<ActualizarEquipoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _equiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarEquipo_Valido_RetornaNoContent()
        {
            int idValido = 5;
            _equipoServiceMock.Setup(s => s.EliminarEquipo(It.Is<EliminarEquipoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _equiposController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarEquipo_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido() };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarEquipo_BadRequest))]
        public void EliminarEquipo_Invalido_RetornaBadRequest(int idEquipo, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.EliminarEquipo(It.Is<EliminarEquipoComando>(c => c.Id == idEquipo))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _equiposController.Eliminar(idEquipo);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarEquipo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _equipoServiceMock.Setup(s => s.EliminarEquipo(It.Is<EliminarEquipoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _equiposController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarEquipo_EnUso_RetornaConflict()
        {
            int idEnUso = 13;
            _equipoServiceMock.Setup(s => s.EliminarEquipo(It.Is<EliminarEquipoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _equiposController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarEquipo_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _equipoServiceMock.Setup(s => s.EliminarEquipo(It.Is<EliminarEquipoComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _equiposController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
