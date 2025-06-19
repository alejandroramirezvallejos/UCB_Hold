using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class UsuarioServiceTest : IUsuarioServiceTest
    {
        private Mock<UsuarioRepository> _usuarioRepositoryMock;
        private UsuarioService          _usuarioService;

        [SetUp]
        public void Setup()
        {
            _usuarioRepositoryMock = new Mock<UsuarioRepository>();
            _usuarioService        = new UsuarioService(_usuarioRepositoryMock.Object);
        }

        [Test]
        public void CrearUsuario_ComandoValido_LlamaRepositorioCrear()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("1", "Andrea", "Vargas", "Rojas", null, "estudiante0@ucb.edu.bo", "password1", "Sistemas", "77327303", "68834902", "Antonio Cruz", "referencia1047@gmail.com");
            _usuarioService.CrearUsuario(comando);
            _usuarioRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearUsuario_CarnetVacio_LanzaErrorNombreRequerido()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("", "Juan", "Perez", "Gomez", null, "juan.perez@ucb.edu.bo", "pass123", "Sistemas", "77712345", null, null, null);
            Assert.Throws<ErrorNombreRequerido>(() => _usuarioService.CrearUsuario(comando));
        }

        [Test]
        public void CrearUsuario_EmailInvalido_LanzaErrorNombreRequerido()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("12345", "Juan", "Perez", "Gomez", null, "email-invalido", "pass123", "Sistemas", "77712345", null, null, null);
            Assert.Throws<ErrorNombreRequerido>(() => _usuarioService.CrearUsuario(comando));
        }

        [Test]
        public void ObtenerTodosUsuarios_CuandoHayDatos_RetornaListaDeDtos()
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

            _usuarioRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(usuariosDataTable);

            List<UsuarioDto> resultado = _usuarioService.ObtenerTodosUsuarios();
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Carnet, Is.EqualTo("1"));
            Assert.That(resultado[1].Carnet, Is.EqualTo("2"));
        }

        [Test]
        public void ActualizarUsuario_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("1", "Andrea Maria", null, null, null, null, null, null, null, null, null, null);
            _usuarioService.ActualizarUsuario(comando);
            _usuarioRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarUsuario_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarUsuarioComando comando = new EliminarUsuarioComando("1");
            _usuarioService.EliminarUsuario(comando);
            _usuarioRepositoryMock.Verify(r => r.Eliminar(comando.Carnet), Times.Once);
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

            UsuarioDto resultado = _usuarioService.IniciarSesionUsuario(consulta);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.Email, Is.EqualTo(consulta.Email));
        }

        [Test]
        public void IniciarSesionUsuario_CredencialesInvalidas_RetornaNull()
        {
            IniciarSesionUsuarioConsulta consulta = new IniciarSesionUsuarioConsulta("fernando.terrazas@ucb.edu.bo", "wrongpass");
            _usuarioRepositoryMock.Setup(r => r.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena)).Returns(new DataTable());

            UsuarioDto resultado = _usuarioService.IniciarSesionUsuario(consulta);
            Assert.IsNull(resultado);
        }
    }
}
