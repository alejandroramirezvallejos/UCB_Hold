using API.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;
using System.Collections.Generic;
using System;

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
            _comentarioServiceMock.Setup(s => s.CrearComentario(comando));
            
            var resultado = _comentarioController.Crear(comando);
            Assert.That(resultado, Is.InstanceOf<CreatedResult>());
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
            _comentarioServiceMock.Setup(s => s.CrearComentario(comando)).Throws(excepcionLanzada);
            
            var resultado = _comentarioController.Crear(comando);
            Assert.That(resultado, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void CrearComentario_ServicioError_RetornaError500()
        {
            var comando = new CrearComentarioComando("2", 6, "Error");
            _comentarioServiceMock.Setup(s => s.CrearComentario(comando)).Throws(new Exception("Error General Servidor"));
            var resultado = _comentarioController.Crear(comando);
            
            Assert.That(resultado, Is.InstanceOf<ObjectResult>());
            var objectResult = resultado as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_ConDatos_RetornaOk()
        {
            var idGrupoEquipo = 8;
            var comentariosEsperados = new List<ComentarioDto>
            {
                new ComentarioDto { Id = "68531f233cba0b4adf2ea2cd", CarnetUsuario = "7", IdGrupoEquipo = 8, Contenido = "El servidor está bien configurado, pero recomendaría actualizar el sis…", Likes = 3, FechaCreacion = DateTime.Parse("2025-06-12T09:15:00.000Z") }
            };
            _comentarioServiceMock.Setup(s => s.ObtenerComentariosPorGrupoEquipo(It.Is<ObtenerComentariosPorGrupoEquipoConsulta>(c => c.IdGrupoEquipo == idGrupoEquipo))).Returns(comentariosEsperados);
            IActionResult resultado = _comentarioController.ObtenerComentariosPorGrupoEquipo(idGrupoEquipo);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(comentariosEsperados));
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_SinDatos_RetornaNotFound()
        {
            var idGrupoEquipo = 999;
            _comentarioServiceMock.Setup(s => s.ObtenerComentariosPorGrupoEquipo(It.IsAny<ObtenerComentariosPorGrupoEquipoConsulta>())).Returns(new List<ComentarioDto>());
            IActionResult resultado = _comentarioController.ObtenerComentariosPorGrupoEquipo(idGrupoEquipo);
            Assert.That(resultado, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void ObtenerComentariosPorGrupoEquipo_IdInvalido_RetornaBadRequest()
        {
            var idGrupoEquipo = 0;
            _comentarioServiceMock.Setup(s => s.ObtenerComentariosPorGrupoEquipo(It.IsAny<ObtenerComentariosPorGrupoEquipoConsulta>())).Throws(new ErrorIdInvalido());
            IActionResult resultado = _comentarioController.ObtenerComentariosPorGrupoEquipo(idGrupoEquipo);
            Assert.That(resultado, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarComentario_Valido_RetornaNoContent()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            _comentarioServiceMock.Setup(s => s.EliminarComentario(It.Is<EliminarComentarioComando>(c => c.Id == idComentario)));
            var resultado = _comentarioController.Eliminar(idComentario);

            Assert.That(resultado, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void EliminarComentario_IdInvalido_RetornaBadRequest()
        {
            var idComentario = "id_invalido";
            _comentarioServiceMock.Setup(s => s.EliminarComentario(It.Is<EliminarComentarioComando>(c => c.Id == idComentario))).Throws(new ErrorIdInvalido());
            var resultado = _comentarioController.Eliminar(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EliminarComentario_NoEncontrado_RetornaNotFound()
        {
            var idComentario = "id_no_existente";
            _comentarioServiceMock.Setup(s => s.EliminarComentario(It.IsAny<EliminarComentarioComando>())).Throws(new ErrorRegistroNoEncontrado());
            var resultado = _comentarioController.Eliminar(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void EliminarComentario_NoAutorizado_RetornaForbid()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            _comentarioServiceMock.Setup(s => s.EliminarComentario(It.Is<EliminarComentarioComando>(c => c.Id == idComentario))).Throws(new ErrorUsuarioNoAutorizado());
            var resultado = _comentarioController.Eliminar(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public void AgregarMeGusta_Valido_RetornaOk()
        {
            var idComentario = "68531f233cba0b4adf2ea2cc";
            _comentarioServiceMock.Setup(s => s.AgregarLikeComentario(It.Is<AgregarLikeComentarioComando>(c => c.Id == idComentario)));
            var resultado = _comentarioController.AgregarMeGusta(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void AgregarMeGusta_IdInvalido_RetornaBadRequest()
        {
            var idComentario = "id_invalido";
            _comentarioServiceMock.Setup(s => s.AgregarLikeComentario(It.Is<AgregarLikeComentarioComando>(c => c.Id == idComentario))).Throws(new ErrorIdInvalido());
            var resultado = _comentarioController.AgregarMeGusta(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AgregarMeGusta_NoEncontrado_RetornaNotFound()
        {
            var idComentario = "id_no_existente";
            _comentarioServiceMock.Setup(s => s.AgregarLikeComentario(It.IsAny<AgregarLikeComentarioComando>())).Throws(new ErrorRegistroNoEncontrado());
            var resultado = _comentarioController.AgregarMeGusta(idComentario);
            
            Assert.That(resultado, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
