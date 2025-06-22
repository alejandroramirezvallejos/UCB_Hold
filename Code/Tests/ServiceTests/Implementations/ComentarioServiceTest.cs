using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class ComentarioServiceTest
    {
        private Mock<IComentarioRepository> _comentarioRepositoryMock;
        private Mock<IExecuteQuery> _executeQueryMock;
        private ComentarioService _comentarioService;

        [SetUp]
        public void Setup()
        {
            _comentarioRepositoryMock = new Mock<IComentarioRepository>();
            _executeQueryMock = new Mock<IExecuteQuery>();
            _comentarioService = new ComentarioService(_comentarioRepositoryMock.Object, _executeQueryMock.Object);
        }

        [Test]
        public void CrearComentario_ComandoValido_LlamaRepositorioCrear()
        {
            var comando = new CrearComentarioComando("2", 6, "El router funciona perfectamente, buena velocidad de conexión.");
            _comentarioService.CrearComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearComentario_CarnetUsuarioVacio_LanzaErrorCarnetInvalido()
        {
            var comando = new CrearComentarioComando("", 6, "Contenido");
            Assert.Throws<ErrorCarnetInvalido>(() => _comentarioService.CrearComentario(comando));
        }
        
        [Test]
        public void CrearComentario_IdGrupoEquipoInvalido_LanzaErrorIdInvalido()
        {
            var comando = new CrearComentarioComando("2", 0, "Contenido");
            Assert.Throws<ErrorIdInvalido>(() => _comentarioService.CrearComentario(comando));
        }

        [Test]
        public void CrearComentario_ContenidoVacio_LanzaErrorCampoRequerido()
        {
            var comando = new CrearComentarioComando("2", 6, "");
            Assert.Throws<ErrorCampoRequerido>(() => _comentarioService.CrearComentario(comando));
        }
        
        [Test]
        public void CrearComentario_ContenidoExcedeLimite_LanzaErrorLongitudInvalida()
        {
            var comando = new CrearComentarioComando("2", 6, new string('a', 501));
            Assert.Throws<ErrorLongitudInvalida>(() => _comentarioService.CrearComentario(comando));
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
            
            comentariosDataTable.Rows.Add("68531f233cba0b4adf2ea2cd", "7", 8, "El servidor está bien configurado, pero recomendaría actualizar el sis…", 3, DateTime.Parse("2025-06-12T09:15:00.000Z"));

            var usuariosDataTable = new DataTable();
            usuariosDataTable.Columns.Add("carnet", typeof(string));
            usuariosDataTable.Columns.Add("nombre", typeof(string));
            usuariosDataTable.Columns.Add("apellido_paterno", typeof(string));
            usuariosDataTable.Rows.Add("7", "Test", "User");

            _comentarioRepositoryMock.Setup(r => r.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo)).Returns(comentariosDataTable);
            _executeQueryMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>())).Returns(usuariosDataTable);

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
            _comentarioService.EliminarComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void EliminarComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new EliminarComentarioComando("");
            Assert.Throws<ErrorIdInvalido>(() => _comentarioService.EliminarComentario(comando));
        }

        [Test]
        public void AgregarLikeComentario_ComandoValido_LlamaRepositorioAgregarLike()
        {
            var comando = new AgregarLikeComentarioComando("68531f233cba0b4adf2ea2cc", "2");
            _comentarioService.AgregarLikeComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.AgregarLike(comando), Times.Once);
        }

        [Test]
        public void AgregarLikeComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new AgregarLikeComentarioComando("", "2");
            Assert.Throws<ErrorIdInvalido>(() => _comentarioService.AgregarLikeComentario(comando));
        }
    }
}
