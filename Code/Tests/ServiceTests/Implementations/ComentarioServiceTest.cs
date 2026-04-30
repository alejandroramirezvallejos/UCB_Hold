using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class ComentarioServiceTest
    {
        private Mock<IComentarioRepository> _comentarioRepositoryMock;
        private Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private ComentarioService _comentarioService;

        [SetUp]
        public void Setup()
        {
            _comentarioRepositoryMock = new Mock<IComentarioRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _comentarioService = new ComentarioService(_comentarioRepositoryMock.Object, _usuarioRepositoryMock.Object);
        }

        [Test]
        public void CrearComentario_ComandoValido_LlamaRepositorioCrear()
        {
            var comando = new CrearComentarioComando("2", 6, "El router funciona perfectamente, buena velocidad de conexión.");
            _comentarioRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearComentarioComando>())).Returns(Result<ComentarioDto>.Created(new ComentarioDto()));
            _comentarioService.Crear(comando);
            _comentarioRepositoryMock.Verify(r => r.Crear(It.IsAny<CrearComentarioComando>()), Times.Once);
        }

        [Test]
        public void CrearComentario_CarnetUsuarioVacio_LanzaErrorCarnetInvalido()
        {
            var comando = new CrearComentarioComando("", 6, "Contenido");
            var resultado = _comentarioService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearComentario_IdGrupoEquipoInvalido_LanzaErrorIdInvalido()
        {
            var comando = new CrearComentarioComando("2", 0, "Contenido");
            var resultado = _comentarioService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearComentario_ContenidoVacio_LanzaErrorCampoRequerido()
        {
            var comando = new CrearComentarioComando("2", 6, "");
            var resultado = _comentarioService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearComentario_ContenidoExcedeLimite_LanzaErrorLongitudInvalida()
        {
            var comando = new CrearComentarioComando("2", 6, new string('a', 501));
            var resultado = _comentarioService.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_CuandoHayDatos_RetornaListaDeDtos()
        {
            var consulta = new ObtenerComentariosPorGrupoEquipoConsulta(8);
            var comentariosDataTable = new DataTable();
            comentariosDataTable.Columns.Add("id_comentario", typeof(string));
            comentariosDataTable.Columns.Add("carnet_usuario", typeof(string));
            comentariosDataTable.Columns.Add("id_grupo_equipo", typeof(int));
            comentariosDataTable.Columns.Add("contenido_comentario", typeof(string));
            comentariosDataTable.Columns.Add("likes_comentario", typeof(int));
            comentariosDataTable.Columns.Add("fecha_creacion_comentario", typeof(DateTime));
            comentariosDataTable.Rows.Add("68531f233cba0b4adf2ea2cd", "7", 8, "El servidor está bien configurado.", 3, DateTime.Parse("2025-06-12T09:15:00.000Z"));

            var usuariosDataTable = new DataTable();
            usuariosDataTable.Columns.Add("carnet", typeof(string));
            usuariosDataTable.Columns.Add("nombre", typeof(string));
            usuariosDataTable.Columns.Add("apellido_paterno", typeof(string));
            usuariosDataTable.Rows.Add("7", "Test", "User");

            _comentarioRepositoryMock.Setup(r => r.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo)).Returns(comentariosDataTable);
            _usuarioRepositoryMock.Setup(e => e.ObtenerPorCarnets(It.IsAny<List<string>>())).Returns(usuariosDataTable);

            var resultado = _comentarioService.ObtenerComentariosPorGrupoEquipo(consulta);

            Assert.That(resultado, Has.Count.EqualTo(1));
            Assert.That(resultado[0].Id, Is.EqualTo("68531f233cba0b4adf2ea2cd"));
            Assert.That(resultado[0].CarnetUsuario, Is.EqualTo("7"));
            Assert.That(resultado[0].NombreUsuario, Is.EqualTo("Test"));
            Assert.That(resultado[0].ApellidoPaternoUsuario, Is.EqualTo("User"));
        }

        [Test]
        public void EliminarComentario_ComandoValido_LlamaRepositorioEliminar()
        {
            var comando = new EliminarComentarioComando("68531f233cba0b4adf2ea2cc");
            _comentarioRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarComentarioComando>())).Returns(Result<ComentarioDto>.Success(new ComentarioDto()));
            _comentarioService.Eliminar(comando);
            _comentarioRepositoryMock.Verify(r => r.Eliminar(It.IsAny<EliminarComentarioComando>()), Times.Once);
        }

        [Test]
        public void EliminarComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new EliminarComentarioComando("");
            var resultado = _comentarioService.Eliminar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void AgregarLikeComentario_ComandoValido_LlamaRepositorioAgregarLike()
        {
            var comando = new AgregarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "2");
            _comentarioRepositoryMock.Setup(r => r.AgregarLike(It.IsAny<AgregarLikeComentarioComando>())).Returns(Result<ComentarioDto>.Success(new ComentarioDto()));
            _comentarioService.AgregarLikeComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.AgregarLike(It.IsAny<AgregarLikeComentarioComando>()), Times.Once);
        }

        [Test]
        public void AgregarLikeComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new AgregarLikeComentarioComando("", "2");
            Assert.Throws<ArgumentException>(() => _comentarioService.AgregarLikeComentario(comando));
        }

        [Test]
        public void QuitarLikeComentario_ComandoValido_LlamaRepositorioQuitarLike()
        {
            var comando = new QuitarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "2");
            _comentarioRepositoryMock.Setup(r => r.QuitarLike(It.IsAny<QuitarLikeComentarioComando>())).Returns(Result<ComentarioDto>.Success(new ComentarioDto()));
            _comentarioService.QuitarLikeComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.QuitarLike(It.IsAny<QuitarLikeComentarioComando>()), Times.Once);
        }

        [Test]
        public void QuitarLikeComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new QuitarLikeComentarioComando("", "2");
            Assert.Throws<ArgumentException>(() => _comentarioService.QuitarLikeComentario(comando));
        }

        [Test]
        public void QuitarLikeComentario_CarnetUsuarioVacio_LanzaErrorCarnetInvalido()
        {
            var comando = new QuitarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "");
            Assert.Throws<ArgumentException>(() => _comentarioService.QuitarLikeComentario(comando));
        }
    }
}
