using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Services;
using IMT_Reservas.Server.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class ComentarioServiceTest
    {
        private Mock<IComentarioRepository> _comentarioRepositoryMock;
        private ComentarioService _comentarioService;

        [SetUp]
        public void Setup()
        {
            _comentarioRepositoryMock = new Mock<IComentarioRepository>();
            _comentarioService = new ComentarioService(_comentarioRepositoryMock.Object);
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
            comentariosDataTable.Columns.Add("nombre_usuario", typeof(string));
            comentariosDataTable.Columns.Add("apellido_paterno_usuario", typeof(string));
            comentariosDataTable.Columns.Add("id_grupo_equipo", typeof(string));
            comentariosDataTable.Columns.Add("contenido_comentario", typeof(string));
            comentariosDataTable.Columns.Add("likes_comentario", typeof(int));
            comentariosDataTable.Columns.Add("fecha_creacion_comentario", typeof(DateTime));
            
            comentariosDataTable.Rows.Add("68531f233cba0b4adf2ea2cd", "7", "Test", "User", "8", "El servidor está bien configurado, pero recomendaría actualizar el sis…", 3, DateTime.Parse("2025-06-12T09:15:00.000Z"));

            _comentarioRepositoryMock.Setup(r => r.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo)).Returns(comentariosDataTable);

            var resultado = _comentarioService.ObtenerComentariosPorGrupoEquipo(consulta);
            
            Assert.That(resultado, Has.Count.EqualTo(1));
            Assert.That(resultado[0].Id, Is.EqualTo("68531f233cba0b4adf2ea2cd"));
            Assert.That(resultado[0].CarnetUsuario, Is.EqualTo("7"));
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
            var comando = new AgregarLikeComentarioComando("68531f233cba0b4adf2ea2cc");
            _comentarioService.AgregarLikeComentario(comando);
            _comentarioRepositoryMock.Verify(r => r.AgregarLike(comando), Times.Once);
        }

        [Test]
        public void AgregarLikeComentario_IdInvalido_LanzaErrorIdInvalido()
        {
            var comando = new AgregarLikeComentarioComando("");
            Assert.Throws<ErrorIdInvalido>(() => _comentarioService.AgregarLikeComentario(comando));
        }
    }
}

