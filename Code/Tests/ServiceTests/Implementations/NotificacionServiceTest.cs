using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class NotificacionServiceTest
    {
        private Mock<INotificacionRepository> _notificacionRepositoryMock;
        private NotificacionService _notificacionService;

        [SetUp]
        public void Setup()
        {
            _notificacionRepositoryMock = new Mock<INotificacionRepository>();
            _notificacionService = new NotificacionService(_notificacionRepositoryMock.Object);
        }

        [Test]
        public void CrearNotificacion_ComandoValido_LlamaRepositorioCrear()
        {
            var comando = new CrearNotificacionComando("12890061", "Solicitud aprobada", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada. Pue…");
            _notificacionService.CrearNotificacion(comando);
            _notificacionRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void ObtenerNotificacionesPorUsuario_CuandoHayDatos_RetornaListaDeDtos()
        {
            var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta("12890061");
            var notificacionesDataTable = new DataTable();
            notificacionesDataTable.Columns.Add("id_notificacion", typeof(string));
            notificacionesDataTable.Columns.Add("carnet_usuario", typeof(string));
            notificacionesDataTable.Columns.Add("titulo", typeof(string));
            notificacionesDataTable.Columns.Add("contenido", typeof(string));
            notificacionesDataTable.Columns.Add("fecha_envio", typeof(DateTime));
            notificacionesDataTable.Columns.Add("leido", typeof(bool));

            notificacionesDataTable.Rows.Add("68535f7ddd47665ee70310b7", "12890061", "Solicitud aprobada", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada. Pue…", DateTime.Parse("2025-06-12T09:15:00.000Z"), false);
            notificacionesDataTable.Rows.Add("68535f7ddd47665ee70310b8", "12890061", "Solicitud rechazada", "Tu solicitud de préstamo para Monitor Profesional ha sido rechazada de…", DateTime.Parse("2025-06-14T10:30:00.000Z"), false);

            _notificacionRepositoryMock.Setup(r => r.ObtenerPorUsuario(consulta)).Returns(notificacionesDataTable);

            var resultado = _notificacionService.ObtenerNotificacionesPorUsuario(consulta);

            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Id, Is.EqualTo("68535f7ddd47665ee70310b7"));
            Assert.That(resultado[1].Id, Is.EqualTo("68535f7ddd47665ee70310b8"));
        }

        [Test]
        public void EliminarNotificacion_ComandoValido_LlamaRepositorioEliminar()
        {
            var comando = new EliminarNotificacionComando("68535f7ddd47665ee70310b7");
            _notificacionService.EliminarNotificacion(comando);
            _notificacionRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void MarcarNotificacionComoLeida_ComandoValido_LlamaRepositorioMarcarComoLeida()
        {
            var comando = new MarcarComoLeidoComando("68535f7ddd47665ee70310b7");
            _notificacionService.MarcarNotificacionComoLeida(comando);
            _notificacionRepositoryMock.Verify(r => r.MarcarComoLeida(comando), Times.Once);
        }
    }
}
