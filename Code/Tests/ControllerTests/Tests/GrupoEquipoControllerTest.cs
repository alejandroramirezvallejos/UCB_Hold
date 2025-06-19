using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class GrupoEquipoControllerTest : IGrupoEquipoControllerTest
    {
        private Mock<GrupoEquipoService>    _grupoEquipoServiceMock;
        private Mock<GrupoEquipoRepository> _grupoEquipoRepoMock;
        private Mock<ExecuteQuery>          _queryExecMock;
        private Mock<IConfiguration>        _configMock;
        private GrupoEquipoController       _grupoEquiposController;

        [SetUp]
        public void Setup()
        {
            _configMock             = new Mock<IConfiguration>();
            _configMock.Setup(config => config.GetSection("ConnectionStrings")["DefaultConnection"]).Returns("fake_connection_string");
            _queryExecMock          = new Mock<ExecuteQuery>(_configMock.Object);
            _grupoEquipoRepoMock    = new Mock<GrupoEquipoRepository>(_queryExecMock.Object);
            _grupoEquipoServiceMock = new Mock<GrupoEquipoService>(_grupoEquipoRepoMock.Object);
            _grupoEquiposController = new GrupoEquipoController(_grupoEquipoServiceMock.Object);
        }

        [Test]
        public void GetGruposEquipos_ConDatos_RetornaOk()
        {
            List<GrupoEquipoDto> gruposEsperados = new List<GrupoEquipoDto>
            {
                new GrupoEquipoDto { Id = 1, Nombre = "Impresora" },
                new GrupoEquipoDto { Id = 2, Nombre = "Soldamatics" }
            };
            _grupoEquipoServiceMock.Setup(s => s.ObtenerTodosGruposEquipos()).Returns(gruposEsperados);
            ActionResult<List<GrupoEquipoDto>> resultadoAccion = _grupoEquiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<GrupoEquipoDto>>().And.Count.EqualTo(gruposEsperados.Count));
        }

        [Test]
        public void GetGruposEquipos_SinDatos_RetornaOkVacia()
        {
            List<GrupoEquipoDto> gruposEsperados = new List<GrupoEquipoDto>();
            _grupoEquipoServiceMock.Setup(s => s.ObtenerTodosGruposEquipos()).Returns(gruposEsperados);
            ActionResult<List<GrupoEquipoDto>> resultadoAccion = _grupoEquiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<GrupoEquipoDto>>().And.Empty);
        }

        [Test]
        public void GetGruposEquipos_ServicioError_RetornaBadRequest()
        {
            _grupoEquipoServiceMock.Setup(s => s.ObtenerTodosGruposEquipos()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<GrupoEquipoDto>> resultadoAccion = _grupoEquiposController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearGrupoEquipo_Valido_RetornaCreated()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png");
            _grupoEquipoServiceMock.Setup(s => s.CrearGrupoEquipo(comando));
            IActionResult resultadoAccion = _grupoEquiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearGrupoEquipo_BadRequest()
        {
            yield return new object[] { new CrearGrupoEquipoComando("", "Modelo", "Marca", "Desc", "Cat", null, "img"), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "", "Marca", "Desc", "Cat", null, "img"), new ErrorModeloRequerido() };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "", "Desc", "Cat", null, "img"), new ErrorCampoRequerido("marca") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "", "Cat", null, "img"), new ErrorCampoRequerido("descripcion") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "Desc", "", null, "img"), new ErrorCampoRequerido("categoria") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "Desc", "Cat", null, ""), new ErrorCampoRequerido("url de imagen") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearGrupoEquipo_BadRequest))]
        public void CrearGrupoEquipo_Invalido_RetornaBadRequest(CrearGrupoEquipoComando comando, System.Exception excepcionLanzada)
        {
            _grupoEquipoServiceMock.Setup(s => s.CrearGrupoEquipo(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _grupoEquiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearGrupoEquipo_RegistroExistente_RetornaConflict()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Impresora", "prueba", "prueba", "prueba", "Impresoras", "string", "https://th.bing.com/th/id/OIP.u6bg7Q6XQdd5ZCfumbYt9AHaD4?cb=iwp1&rs=1&pid=ImgDetMain");
            _grupoEquipoServiceMock.Setup(s => s.CrearGrupoEquipo(It.IsAny<CrearGrupoEquipoComando>())).Throws(new ErrorRegistroYaExiste());
            IActionResult resultadoAccion = _grupoEquiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void CrearGrupoEquipo_ServicioError_RetornaError500()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Error", "Error", "Error", "Error", "Error", null, "Error");
            _grupoEquipoServiceMock.Setup(s => s.CrearGrupoEquipo(It.IsAny<CrearGrupoEquipoComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _grupoEquiposController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = (ObjectResult)resultadoAccion;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ActualizarGrupoEquipo_Valido_RetornaOk()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https::prueba-v2", "img_act");
            _grupoEquipoServiceMock.Setup(s => s.ActualizarGrupoEquipo(comando));
            IActionResult resultadoAccion = _grupoEquiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarGrupoEquipo_BadRequest()
        {
            yield return new object[] { new ActualizarGrupoEquipoComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido() };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "", null, null, null, null, null), new ErrorModeloRequerido() };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "", null, null, null, null), new ErrorCampoRequerido("marca") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "", null, null, null), new ErrorCampoRequerido("descripcion") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "Desc", "", null, null), new ErrorCampoRequerido("categoria") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "Desc", "Cat", null, ""), new ErrorCampoRequerido("url de imagen") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarGrupoEquipo_BadRequest))]
        public void ActualizarGrupoEquipo_Invalido_RetornaBadRequest(ActualizarGrupoEquipoComando comando, System.Exception excepcionLanzada)
        {
            _grupoEquipoServiceMock.Setup(s => s.ActualizarGrupoEquipo(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _grupoEquiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarGrupoEquipo_NoEncontrado_RetornaNotFound()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(99, "NoExiste", null, null, null, null, null, null);
            _grupoEquipoServiceMock.Setup(s => s.ActualizarGrupoEquipo(It.IsAny<ActualizarGrupoEquipoComando>())).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _grupoEquiposController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarGrupoEquipo_Valido_RetornaNoContent()
        {
            int idValido = 16;
            _grupoEquipoServiceMock.Setup(s => s.EliminarGrupoEquipo(It.Is<EliminarGrupoEquipoComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _grupoEquiposController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void EliminarGrupoEquipo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _grupoEquipoServiceMock.Setup(s => s.EliminarGrupoEquipo(It.Is<EliminarGrupoEquipoComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado());
            IActionResult resultadoAccion = _grupoEquiposController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarGrupoEquipo_EnUso_RetornaConflict()
        {
            int idEnUso = 1;
            _grupoEquipoServiceMock.Setup(s => s.EliminarGrupoEquipo(It.Is<EliminarGrupoEquipoComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso());
            IActionResult resultadoAccion = _grupoEquiposController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
    }
}
