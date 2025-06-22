using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class UsuarioRepositoryTest : IUsuarioRepositoryTest
    {
        private Mock<IExecuteQuery> _ejecutarConsultaMock;
        private IUsuarioRepository _usuarioRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _usuarioRepositorio   = new UsuarioRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearUsuarioComando comando = new CrearUsuarioComando("1", "Andrea", "Vargas", "Rojas", null, "estudiante0@ucb.edu.bo", "password1", "Sistemas", "77327303", "68834902", "Antonio Cruz", "referencia1047@gmail.com");
            _usuarioRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_usuario")),
                It.Is<Dictionary<string, object?>>(d => (string)d["carnet"] == comando.Carnet)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_usuarios")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _usuarioRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorEmailYContrasena_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Rows.Add(tablaEsperada.NewRow());
            string email = "fernando.terrazas@ucb.edu.bo";
            string contrasena = "123456";
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_usuario_iniciar_sesion")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable? resultado = _usuarioRepositorio.ObtenerPorEmailYContrasena(email, contrasena);
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorEmailYContrasena_CuandoNoHayFilas_RetornaNull()
        {
            DataTable tablaVacia = new DataTable();
            string email = "test@ucb.edu.bo";
            string contrasena = "password";
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaVacia);

            DataTable? resultado = _usuarioRepositorio.ObtenerPorEmailYContrasena(email, contrasena);
            Assert.IsNull(resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarUsuarioComando comando = new ActualizarUsuarioComando("1", "Andrea Maria", null, null, null, null, null, null, null, null, null, null);
            _usuarioRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_usuario")),
                It.Is<Dictionary<string, object?>>(d => (string)d["carnet"] == comando.Carnet)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            string carnet = "1";
            _usuarioRepositorio.Eliminar(carnet);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_usuario")),
                It.Is<Dictionary<string, object?>>(d => (string)d["carnet"] == carnet)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<ErrorRepository>(() => _usuarioRepositorio.Crear(new CrearUsuarioComando("1", "n", "p", "m", null, "e@e.com", "c", "ca", null, null, null, null)));
            Assert.Throws<ErrorRepository>(() => _usuarioRepositorio.Actualizar(new ActualizarUsuarioComando("1", null, null, null, null, null, null, null, null, null, null, null)));
            Assert.Throws<ErrorRepository>(() => _usuarioRepositorio.Eliminar("1"));
            Assert.Throws<ErrorRepository>(() => _usuarioRepositorio.ObtenerTodos());
            Assert.Throws<ErrorRepository>(() => _usuarioRepositorio.ObtenerPorEmailYContrasena("e", "c"));
        }
    }
}
