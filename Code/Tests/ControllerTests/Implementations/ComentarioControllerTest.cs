using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class ComentarioControllerTest
    {
        private Mock<IComentarioService> _comentarioServiceMock;
        private ComentarioController _comentarioController;

        [SetUp]
        public void Setup()
        {
            _comentarioServiceMock = new Mock<IComentarioService>();
            _comentarioController = new ComentarioController(_comentarioServiceMock.Object);
        }

        [Test]
        public void CrearComentario_Valido_RetornaCreated()
        {
            var comando = new CrearComentarioComando("2", 6, "El router funciona perfectamente, buena velocidad de conexión.");
            var dto = new ComentarioDto { Id = "abc123", CarnetUsuario = "2", IdGrupoEquipo = 6 };
            _comentarioServiceMock.Setup(s => s.Crear(It.IsAny<CrearComentarioComando>())).Returns(Result<ComentarioDto>.Created(dto));
            var resultado = _comentarioController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearComentario_BadRequest()
        {
            yield return new object[] { new CrearComentarioComando("", 6, "Contenido"), new ErrorCarnetInvalido() };
            yield return new object[] { new CrearComentarioComando("2", 0, "Contenido"), new ErrorIdInvalido("El IdGrupoEquipo es requerido y debe ser mayor a 0.") };
            yield return new object[] { new CrearComentarioComando("2", 6, ""), new ErrorCampoRequerido("contenido") };
            yield return new object[] { new CrearComentarioComando("2", 6, new string('a', 501)), new ErrorLongitudInvalida("contenido", 500) };
            yield return new object[] { new CrearComentarioComando("usuario_invalido", 6, "Contenido"), new ErrorReferenciaInvalida("usuario") };
            yield return new object[] { new CrearComentarioComando("2", 999, "Contenido"), new ErrorReferenciaInvalida("grupo de equipos") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearComentario_BadRequest))]
        public void CrearComentario_Invalido_RetornaBadRequest(CrearComentarioComando comando, System.Exception excepcionLanzada)
        {
            _comentarioServiceMock.Setup(s => s.Crear(It.IsAny<CrearComentarioComando>())).Returns(Result<ComentarioDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _comentarioController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearComentario_ServicioError_RetornaError()
        {
            var comando = new CrearComentarioComando("2", 6, "Error");
            _comentarioServiceMock.Setup(s => s.Crear(It.IsAny<CrearComentarioComando>())).Returns(Result<ComentarioDto>.Error("Error General Servidor"));
            var resultado = _comentarioController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_ConDatos_RetornaOk()
        {
            var idGrupoEquipo = 8;
            var comentariosEsperados = new List<ComentarioDto>
            {
                new ComentarioDto { Id = "68531f233cba0b4adf2ea2cd", CarnetUsuario = "7", IdGrupoEquipo = 8, Contenido = "El servidor está bien configurado.", Likes = 3, FechaCreacion = DateTime.Parse("2025-06-12T09:15:00.000Z") }
            };
            _comentarioServiceMock.Setup(s => s.ObtenerComentariosPorGrupoEquipo(It.Is<ObtenerComentariosPorGrupoEquipoConsulta>(c => c.IdGrupoEquipo == idGrupoEquipo))).Returns(comentariosEsperados);
            var resultado = _comentarioController.ObtenerComentariosPorGrupoEquipo(idGrupoEquipo);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(comentariosEsperados));
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_SinDatos_RetornaNotFound()
        {
            var idGrupoEquipo = 999;
            _comentarioServiceMock.Setup(s => s.ObtenerComentariosPorGrupoEquipo(It.IsAny<ObtenerComentariosPorGrupoEquipoConsulta>())).Returns(new List<ComentarioDto>());
            var resultado = _comentarioController.ObtenerComentariosPorGrupoEquipo(idGrupoEquipo);
            Assert.That(resultado, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarComentario_Valido_RetornaSuccess()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            var dto = new ComentarioDto { Id = idComentario };
            _comentarioServiceMock.Setup(s => s.Eliminar(It.Is<EliminarComentarioComando>(c => c.Id == idComentario))).Returns(Result<ComentarioDto>.Success(dto));
            var resultado = _comentarioController.Eliminar(idComentario);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void EliminarComentario_IdInvalido_RetornaInvalid()
        {
            var idComentario = "id_invalido";
            _comentarioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComentarioComando>())).Returns(Result<ComentarioDto>.Invalid(new ValidationError("Id", "Id inválido")));
            var resultado = _comentarioController.Eliminar(idComentario);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarComentario_NoEncontrado_RetornaNotFound()
        {
            var idComentario = "id_no_existente";
            _comentarioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComentarioComando>())).Returns(Result<ComentarioDto>.NotFound());
            var resultado = _comentarioController.Eliminar(idComentario);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarComentario_NoAutorizado_RetornaForbidden()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            _comentarioServiceMock.Setup(s => s.Eliminar(It.Is<EliminarComentarioComando>(c => c.Id == idComentario))).Returns(Result<ComentarioDto>.Forbidden());
            var resultado = _comentarioController.Eliminar(idComentario);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Forbidden));
        }

        [Test]
        public void AgregarMeGusta_Valido_RetornaOk()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            var carnetUsuario = "2";
            var comando = new AgregarLikeComentarioComando(idComentario, carnetUsuario);
            _comentarioServiceMock.Setup(s => s.AgregarLikeComentario(It.IsAny<AgregarLikeComentarioComando>()));
            var resultado = _comentarioController.AgregarMeGusta(idComentario, comando);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void QuitarMeGusta_Valido_RetornaOk()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            var carnetUsuario = "2";
            var comando = new QuitarLikeComentarioComando(idComentario, carnetUsuario);
            _comentarioServiceMock.Setup(s => s.QuitarLikeComentario(It.IsAny<QuitarLikeComentarioComando>()));
            var resultado = _comentarioController.QuitarMeGusta(idComentario, comando);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }
    }
}
