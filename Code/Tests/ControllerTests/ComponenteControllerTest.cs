using API.Controllers;
using Moq;
using Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class ComponenteControllerTest
    {
        private Mock<ComponenteService> _componenteServiceMock;
        private Mock<ComponenteRepository> _componenteRepoMock;
        private Mock<ExecuteQuery> _queryExecMock;
        private Mock<IConfiguration> _configMock;
        private ComponenteController _componentesController;

        [SetUp]
        public void Setup()
        {
            _configMock            = new Mock<IConfiguration>();
            _queryExecMock         = new Mock<ExecuteQuery>(_configMock.Object);
            _componenteRepoMock    = new Mock<ComponenteRepository>(_queryExecMock.Object);
            _componenteServiceMock = new Mock<ComponenteService>(_componenteRepoMock.Object);
            _componentesController = new ComponenteController(_componenteServiceMock.Object);
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
        }

        [Test]
        public void GetComponentes_ConDatos_RetornaOk()
        {
            List<ComponenteDto> componentesEsperados = new List<ComponenteDto>
            {
                new ComponenteDto { Id = 1, Nombre = "CPU Intel i7", Modelo = "SR123" },
                new ComponenteDto { Id = 2, Nombre = "RAM 16GB DDR4", Modelo = "RM456" }
            };
            _componenteServiceMock.Setup(s => s.ObtenerTodosComponentes()).Returns(componentesEsperados);
            ActionResult<List<ComponenteDto>> resultadoAccion = _componentesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<ComponenteDto>>().And.Count.EqualTo(componentesEsperados.Count));
        }

        [Test]
        public void GetComponentes_SinDatos_RetornaOkVacia()
        {
            List<ComponenteDto> componentesEsperados = new List<ComponenteDto>();
            _componenteServiceMock.Setup(s => s.ObtenerTodosComponentes()).Returns(componentesEsperados);
            ActionResult<List<ComponenteDto>> resultadoAccion = _componentesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion.Result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<ComponenteDto>>().And.Empty);
        }

        [Test]
        public void GetComponentes_ServicioError_RetornaBadRequest()
        {
            _componenteServiceMock.Setup(s => s.ObtenerTodosComponentes()).Throws(new System.Exception("Error servicio"));
            ActionResult<List<ComponenteDto>> resultadoAccion = _componentesController.ObtenerTodos();
            Assert.That(resultadoAccion.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearComponente_Valido_RetornaCreated()
        {
            CrearComponenteComando comando = new CrearComponenteComando("SSD 1TB", "SN789", "Almacenamiento", 1, "SSD NVMe Gen4", 150.00, "http://example.com/datasheet.pdf");
            _componenteServiceMock.Setup(s => s.CrearComponente(comando));
            IActionResult resultadoAccion = _componentesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<CreatedResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_CrearComponente_BadRequest()
        {
            yield return new object[] { new CrearComponenteComando("", "SN1", "Tipo", 1, "desc", 10, null), new ErrorNombreRequerido("nombre") };
            yield return new object[] { new CrearComponenteComando(new string('a', 101), "SN2", "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new CrearComponenteComando("CPU", "", "Tipo", 1, "desc", 10, null), new ErrorNombreRequerido("modelo") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearComponente_BadRequest))]
        public void CrearComponente_Invalido_RetornaBadRequest(CrearComponenteComando comando, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.CrearComponente(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _componentesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearComponente_RegistroExistente_RetornaConflict()
        {
            CrearComponenteComando comando = new CrearComponenteComando("RAM 8GB", "SR123", "Memoria", 2, "desc", 70, null);
            _componenteServiceMock.Setup(s => s.CrearComponente(It.IsAny<CrearComponenteComando>())).Throws(new ErrorRegistroYaExiste("Componente con esta serie ya existe"));
            IActionResult resultadoAccion = _componentesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }
        
        [Test]
        public void CrearComponente_ServicioError_RetornaError500()
        {
            CrearComponenteComando comando = new CrearComponenteComando("Error General", "ERR001", "Error", 9, "desc", 0, null);
            _componenteServiceMock.Setup(s => s.CrearComponente(It.IsAny<CrearComponenteComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _componentesController.Crear(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void ActualizarComponente_Valido_RetornaOk()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "CPU Intel i9", "SR999", "Procesador", 1, "CPU de alta gama", 500.00, "http://example.com/datasheet.pdf");
            _componenteServiceMock.Setup(s => s.ActualizarComponente(comando));
            IActionResult resultadoAccion = _componentesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarComponente_BadRequest()
        {
            yield return new object[] { new ActualizarComponenteComando(0, "Inválido", null, null, 0, null, 0, null), new ErrorIdInvalido("ID") };
            yield return new object[] { new ActualizarComponenteComando(1, "", "SN1", "Tipo", 1, "desc", 10, null), new ErrorNombreRequerido("nombre") };
            yield return new object[] { new ActualizarComponenteComando(1, new string('a', 101), "SN2", "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("nombre", 100) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarComponente_BadRequest))]
        public void ActualizarComponente_Invalido_RetornaBadRequest(ActualizarComponenteComando comando, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.ActualizarComponente(comando)).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _componentesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActualizarComponente_NoEncontrado_RetornaNotFound()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(99, "NoExiste", "NE001", null, 0, null, 0, null); 
            _componenteServiceMock.Setup(s => s.ActualizarComponente(It.IsAny<ActualizarComponenteComando>())).Throws(new ErrorRegistroNoEncontrado("Componente no encontrado"));
            IActionResult resultadoAccion = _componentesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActualizarComponente_RegistroExistente_RetornaConflict()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "RAM 8GB", "RM456", "Memoria", 2, "desc", 70, null);
            _componenteServiceMock.Setup(s => s.ActualizarComponente(It.IsAny<ActualizarComponenteComando>())).Throws(new ErrorRegistroYaExiste("Componente con esta serie ya existe"));
            IActionResult resultadoAccion = _componentesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void ActualizarComponente_ServicioError_RetornaError500()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "Error General", "ERR002", null, 0, null, 0, null);
            _componenteServiceMock.Setup(s => s.ActualizarComponente(It.IsAny<ActualizarComponenteComando>())).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _componentesController.Actualizar(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void EliminarComponente_Valido_RetornaNoContent()
        {
            int idValido = 1;
            _componenteServiceMock.Setup(s => s.EliminarComponente(It.Is<EliminarComponenteComando>(c => c.Id == idValido)));
            IActionResult resultadoAccion = _componentesController.Eliminar(idValido);
            Assert.That(resultadoAccion, Is.InstanceOf<NoContentResult>());
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarComponente_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("ID") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarComponente_BadRequest))]
        public void EliminarComponente_Invalido_RetornaBadRequest(int idComponente, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.EliminarComponente(It.Is<EliminarComponenteComando>(c => c.Id == idComponente))).Throws(excepcionLanzada);
            IActionResult resultadoAccion = _componentesController.Eliminar(idComponente);
            Assert.That(resultadoAccion, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarComponente_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _componenteServiceMock.Setup(s => s.EliminarComponente(It.Is<EliminarComponenteComando>(c => c.Id == idNoExistente))).Throws(new ErrorRegistroNoEncontrado("Componente no encontrado"));
            IActionResult resultadoAccion = _componentesController.Eliminar(idNoExistente);
            Assert.That(resultadoAccion, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarComponente_EnUso_RetornaConflict() 
        {
            int idEnUso = 2;
            _componenteServiceMock.Setup(s => s.EliminarComponente(It.Is<EliminarComponenteComando>(c => c.Id == idEnUso))).Throws(new ErrorRegistroEnUso("Componente en uso, no se puede eliminar"));
            IActionResult resultadoAccion = _componentesController.Eliminar(idEnUso);
            Assert.That(resultadoAccion, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public void EliminarComponente_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _componenteServiceMock.Setup(s => s.EliminarComponente(It.Is<EliminarComponenteComando>(c => c.Id == idErrorGeneral))).Throws(new System.Exception("Error General Servidor"));
            IActionResult resultadoAccion = _componentesController.Eliminar(idErrorGeneral);
            Assert.That(resultadoAccion, Is.InstanceOf<ObjectResult>());
            ObjectResult objectResult = resultadoAccion as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
