using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class UsuarioServiceTest
    {
        private Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private UsuarioService _usuarioService;

        [SetUp]
        public void Setup()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("1", "Andrea", "Vargas", "Rojas", "estudiante", "estudiante0@ucb.edu.bo", "password1", "Sistemas", "77327303", "68834902", "Antonio Cruz", "referencia1047@gmail.com");
            _usuarioRepositoryMock.Setup(r => r.ObtenerCarreraIdPorNombre(It.IsAny<string>())).Returns(1);
            _usuarioRepositoryMock.Setup(r => r.Crear(It.IsAny<int>(), It.IsAny<CrearUsuarioComando>())).Returns(Result<UsuarioDto>.Created(new UsuarioDto { Carnet = "1", Nombre = "Andrea" }));

            var resultado = _usuarioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _usuarioRepositoryMock.Verify(r => r.Crear(It.IsAny<int>(), comando), Times.Once);
        }

        [Test]
        public void Crear_CarnetVacio_RetornaInvalid()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("", "Juan", "Perez", "Gomez", null, "juan.perez@ucb.edu.bo", "pass123", "Sistemas", "77712345", null, null, null);

            var resultado = _usuarioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_EmailInvalido_RetornaInvalid()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("12345", "Juan", "Perez", "Gomez", null, "email-invalido", "pass123", "Sistemas", "77712345", null, null, null);

            var resultado = _usuarioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable usuariosDataTable = new DataTable();
            usuariosDataTable.Columns.Add("carnet", typeof(string));
            usuariosDataTable.Columns.Add("nombre", typeof(string));
            usuariosDataTable.Columns.Add("apellido_paterno", typeof(string));
            usuariosDataTable.Columns.Add("apellido_materno", typeof(string));
            usuariosDataTable.Columns.Add("carrera", typeof(string));
            usuariosDataTable.Columns.Add("rol", typeof(string));
            usuariosDataTable.Columns.Add("email", typeof(string));
            usuariosDataTable.Columns.Add("telefono", typeof(string));
            usuariosDataTable.Columns.Add("telefono_referencia", typeof(string));
            usuariosDataTable.Columns.Add("nombre_referencia", typeof(string));
            usuariosDataTable.Columns.Add("email_referencia", typeof(string));
            usuariosDataTable.Rows.Add("1", "Andrea", "Vargas", "Rojas", "Sistemas", "estudiante", "estudiante0@ucb.edu.bo", "77327303", "68834902", "Antonio Cruz", "referencia1047@gmail.com");
            usuariosDataTable.Rows.Add("2", "Juan", "Silva", "Morales", "Sistemas", "estudiante", "estudiante1@ucb.edu.bo", "78660046", "67152605", "Luis Rojas", "referencia6609@gmail.com");

            _usuarioRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(usuariosDataTable));

            var resultado = _usuarioService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(2));
            Assert.That(resultado.Value[0].Carnet, Is.EqualTo("1"));
            Assert.That(resultado.Value[1].Carnet, Is.EqualTo("2"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("1", "Andrea Maria", null, null, null, null, "estudiante", null, null, null, null, null);
            _usuarioRepositoryMock.Setup(r => r.ExisteActivoPorCarnet(It.IsAny<string>())).Returns(true);
            _usuarioRepositoryMock.Setup(r => r.Actualizar(It.IsAny<int?>(), It.IsAny<ActualizarUsuarioComando>())).Returns(Result<UsuarioDto>.Success(new UsuarioDto { Carnet = "1" }));

            var resultado = _usuarioService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _usuarioRepositoryMock.Verify(r => r.Actualizar(It.IsAny<int?>(), comando), Times.Once);
        }

        [Test]
        public void Actualizar_CarnetVacio_RetornaInvalid()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("", "Andrea Maria", null, null, null, null, "estudiante", null, null, null, null, null);

            var resultado = _usuarioService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarUsuarioComando comando = new EliminarUsuarioComando("1");
            _usuarioRepositoryMock.Setup(r => r.ExisteActivoPorCarnet(It.IsAny<string>())).Returns(true);
            _usuarioRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarUsuarioComando>())).Returns(Result<UsuarioDto>.Success(new UsuarioDto { Carnet = "1" }));

            var resultado = _usuarioService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _usuarioRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_CarnetVacio_RetornaInvalid()
        {
            EliminarUsuarioComando comando = new EliminarUsuarioComando("");

            var resultado = _usuarioService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void IniciarSesionUsuario_CredencialesValidas_RetornaUsuarioDto()
        {
            IniciarSesionUsuarioConsulta consulta = new IniciarSesionUsuarioConsulta("fernando.terrazas@ucb.edu.bo", "123456");
            DataTable dt = new DataTable();
            dt.Columns.Add("carnet");
            dt.Columns.Add("nombre");
            dt.Columns.Add("apellido_paterno");
            dt.Columns.Add("apellido_materno");
            dt.Columns.Add("carrera");
            dt.Columns.Add("rol");
            dt.Columns.Add("email");
            dt.Columns.Add("telefono");
            dt.Columns.Add("telefono_referencia");
            dt.Columns.Add("nombre_referencia");
            dt.Columns.Add("email_referencia");
            dt.Rows.Add("12890061", "FERRR", "Terrazas", "Llanos", "Ingenieria de Sistemas", "administrador", "fernando.terrazas@ucb.edu.bo", "79943071", null, null, null);

            _usuarioRepositoryMock.Setup(r => r.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena)).Returns(dt);

            UsuarioDto? resultado = _usuarioService.IniciarSesionUsuario(consulta);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.Email, Is.EqualTo(consulta.Email));
        }

        [Test]
        public void IniciarSesionUsuario_CredencialesInvalidas_RetornaNull()
        {
            IniciarSesionUsuarioConsulta consulta = new IniciarSesionUsuarioConsulta("fernando.terrazas@ucb.edu.bo", "wrongpass");
            _usuarioRepositoryMock.Setup(r => r.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena)).Returns(new DataTable());

            UsuarioDto? resultado = _usuarioService.IniciarSesionUsuario(consulta);
            Assert.IsNull(resultado);
        }
    }
}
