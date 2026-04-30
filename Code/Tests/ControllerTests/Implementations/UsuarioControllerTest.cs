using Moq;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class UsuarioControllerTest
    {
        private Mock<IUsuarioService> _usuarioServiceMock;
        private UsuarioController _usuariosController;

        [SetUp]
        public void Setup()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _usuariosController = new UsuarioController(_usuarioServiceMock.Object);
        }

        [Test]
        public void GetUsuarios_ConDatos_RetornaOk()
        {
            var usuariosEsperados = new List<UsuarioDto>
            {
                new UsuarioDto { Carnet = "1", Nombre = "Andrea" },
                new UsuarioDto { Carnet = "2", Nombre = "Juan" }
            };
            _usuarioServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<UsuarioDto>>.Success(usuariosEsperados));
            var resultado = _usuariosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(usuariosEsperados.Count));
        }

        [Test]
        public void CrearUsuario_Valido_RetornaOk()
        {
            var comando = new CrearUsuarioComando("3", "Juan", "Moreno", "Silva", null, "estudiante2@ucb.edu.bo", "password3", "Sistemas", "72742435", "61300599", "Lucia Herrera", "referencia7529@gmail.com");
            var dto = new UsuarioDto { Carnet = "3", Nombre = "Juan" };
            _usuarioServiceMock.Setup(s => s.Crear(It.IsAny<CrearUsuarioComando>())).Returns(Result<UsuarioDto>.Created(dto));
            var resultado = _usuariosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        [Test]
        public void ActualizarUsuario_Valido_RetornaOk()
        {
            var comando = new ActualizarUsuarioComando("1", "Andrea Maria", null, null, null, null, null, null, null, null, null, null);
            var dto = new UsuarioDto { Carnet = "1", Nombre = "Andrea Maria" };
            _usuarioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarUsuarioComando>())).Returns(Result<UsuarioDto>.Success(dto));
            var resultado = _usuariosController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void EliminarUsuario_Valido_RetornaOk()
        {
            string carnetValido = "1";
            var dto = new UsuarioDto { Carnet = carnetValido };
            _usuarioServiceMock.Setup(s => s.Eliminar(It.Is<EliminarUsuarioComando>(c => c.Carnet == carnetValido))).Returns(Result<UsuarioDto>.Success(dto));
            var resultado = _usuariosController.Eliminar(carnetValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void IniciarSesion_Valido_RetornaOkConUsuario()
        {
            string email = "fernando.terrazas@ucb.edu.bo";
            string contrasena = "123456";
            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            var usuarioEsperado = new UsuarioDto { Email = email, Nombre = "FERRR" };
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.Is<IniciarSesionUsuarioConsulta>(c => c.Email == email && c.Contrasena == contrasena))).Returns(usuarioEsperado);
            var resultadoAccion = _usuariosController.IniciarSesion(consulta);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)resultadoAccion;
            Assert.That(((UsuarioDto)okResult.Value).Email, Is.EqualTo(usuarioEsperado.Email));
        }

        [Test]
        public void IniciarSesion_Invalido_RetornaOkNulo()
        {
            string email = "fernando.terrazas@ucb.edu.bo";
            string contrasena = "wrongpass";
            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            _usuarioServiceMock.Setup(s => s.IniciarSesionUsuario(It.IsAny<IniciarSesionUsuarioConsulta>())).Returns((UsuarioDto?)null);
            var resultadoAccion = _usuariosController.IniciarSesion(consulta);
            Assert.That(resultadoAccion, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)resultadoAccion;
            Assert.That(okResult.Value, Is.Null);
        }
    }
}
