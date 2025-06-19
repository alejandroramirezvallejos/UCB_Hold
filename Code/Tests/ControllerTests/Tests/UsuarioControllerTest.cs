using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Interfaces;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class UsuarioControllerTest : IUsuarioControllerTest
    {
        private Mock<IUsuarioService>    _usuarioServiceMock;
        private UsuarioController       _usuariosController;

        [SetUp]
        public void Setup()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _usuariosController = new UsuarioController(_usuarioServiceMock.Object);
        }

        [Test]
        public void GetUsuarios_ConDatos_RetornaOk()
        {
            List<UsuarioDto> usuariosEsperados = new List<UsuarioDto>
            {
                new UsuarioDto { Carnet = "1", Nombre = "Andrea" },
                new UsuarioDto { Carnet = "2", Nombre = "Juan" }
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
            CrearUsuarioComando comando = new CrearUsuarioComando("3", "Juan", "Moreno", "Silva", null, "estudiante2@ucb.edu.bo", "password3", "Sistemas", "72742435", "61300599", "Lucia Herrera", "referencia7529@gmail.com");
            _usuarioServiceMock.Setup(s => s.CrearUsuario(comando));
            IActionResult resultadoAccion = _usuariosController.CrearUsuario(comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void ActualizarUsuario_Valido_RetornaOk()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("1", "Andrea Maria", null, null, null, null, null, null, null, null, null, null);
            _usuarioServiceMock.Setup(s => s.ActualizarUsuario(comando));
            IActionResult resultadoAccion = _usuariosController.ActualizarUsuario(comando.Carnet, comando);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void EliminarUsuario_Valido_RetornaOk()
        {
            string carnetValido = "1";
            _usuarioServiceMock.Setup(s => s.EliminarUsuario(It.Is<EliminarUsuarioComando>(c => c.Carnet == carnetValido)));
            IActionResult resultadoAccion = _usuariosController.EliminarUsuario(carnetValido);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void IniciarSesion_Valido_RetornaOkConUsuario()
        {
            string email = "fernando.terrazas@ucb.edu.bo";
            string contrasena = "123456";
            UsuarioDto usuarioEsperado = new UsuarioDto { Email = email, Nombre = "FERRR" };
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.Is<IniciarSesionUsuarioConsulta>(c => c.Email == email && c.Contrasena == contrasena))).Returns(usuarioEsperado);
            IActionResult resultadoAccion = _usuariosController.IniciarSesion(email, contrasena);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(((UsuarioDto)okObjectResult.Value).Email, Is.EqualTo(usuarioEsperado.Email));
        }

        [Test]
        public void IniciarSesion_Invalido_RetornaOkNulo()
        {
            string email = "fernando.terrazas@ucb.edu.bo";
            string contrasena = "wrongpass";
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.IsAny<IniciarSesionUsuarioConsulta>())).Returns((UsuarioDto)null);
            IActionResult resultadoAccion = _usuariosController.IniciarSesion(email, contrasena);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)resultadoAccion;
            Assert.That(okObjectResult.Value, Is.Null);
        }
    }
}
