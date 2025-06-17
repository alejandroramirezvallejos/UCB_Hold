using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class UsuarioControllerTest
    {
        private Mock<UsuarioService>    _usuarioServiceMock;
        private Mock<UsuarioRepository> _usuarioRepoMock;
        private Mock<ExecuteQuery>      _queryExecMock;
        private Mock<IConfiguration>    _configMock;
        private UsuarioController       _usuariosController;

        [SetUp]
        public void Setup()
        {
            _configMock         = new Mock<IConfiguration>();
            _queryExecMock      = new Mock<ExecuteQuery>(_configMock.Object);
            _usuarioRepoMock    = new Mock<UsuarioRepository>(_queryExecMock.Object);
            _usuarioServiceMock = new Mock<UsuarioService>(_usuarioRepoMock.Object);
            _usuariosController = new UsuarioController(_usuarioServiceMock.Object);
            _configMock.Setup(config => config.GetConnectionString("DefaultConnection")).Returns("fake_connection_string");
        }

        [Test]
        public void GetUsuarios_ConDatos_RetornaOk()
        {
            List<UsuarioDto> usuariosEsperados = new List<UsuarioDto>
            {
                new UsuarioDto { Carnet = "12345", Nombre = "Juan" },
                new UsuarioDto { Carnet = "67890", Nombre = "Ana" }
            };
            _usuarioServiceMock.Setup(s => s.ObtenerTodosUsuarios()).Returns(usuariosEsperados);
            IActionResult resultadoAccion = _usuariosController.ObtenerUsuarios();
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<UsuarioDto>>().And.Count.EqualTo(usuariosEsperados.Count));
        }

        [Test]
        public void CrearUsuario_Valido_RetornaOk()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("11111", "Pedro", "Perez", "Paz", "pedro@ucb.edu.bo", "pass123", "Sistemas", "77777777", null, null, null);
            _usuarioServiceMock.Setup(s => s.CrearUsuario(comando));
            IActionResult resultadoAccion = _usuariosController.CrearUsuario(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void ActualizarUsuario_Valido_RetornaOk()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("11111", "Pedro Raul", null, null, null, null, null, null, null, null, null, null);
            _usuarioServiceMock.Setup(s => s.ActualizarUsuario(comando));
            IActionResult resultadoAccion = _usuariosController.ActualizarUsuario(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void EliminarUsuario_Valido_RetornaOk()
        {
            string carnetValido = "11111";
            _usuarioServiceMock.Setup(s => s.EliminarUsuario(It.Is<EliminarUsuarioComando>(c => c.Carnet == carnetValido)));
            IActionResult resultadoAccion = _usuariosController.EliminarUsuario(carnetValido);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void IniciarSesion_Valido_RetornaOkConUsuario()
        {
            string email = "test@ucb.edu.bo";
            string contrasena = "pass123";
            UsuarioDto usuarioEsperado = new UsuarioDto { Email = email, Nombre = "Test" };
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.Is<IniciarSesionUsuarioConsulta>(c => c.Email == email && c.Contrasena == contrasena))).Returns(usuarioEsperado);
            IActionResult resultadoAccion = _usuariosController.IniciarSesion(email, contrasena);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.EqualTo(usuarioEsperado));
        }

        [Test]
        public void IniciarSesion_Invalido_RetornaOkNulo()
        {
            string email = "wrong@ucb.edu.bo";
            string contrasena = "wrongpass";
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.IsAny<IniciarSesionUsuarioConsulta>())).Returns((UsuarioDto)null);
            IActionResult resultadoAccion = _usuariosController.IniciarSesion(email, contrasena);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.Null);
        }
    }
}
