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
    public class NotificacionControllerTest
    {
        private Mock<INotificacionService> _notificacionServiceMock;
        private NotificacionController _notificacionController;

        [SetUp]
        public void Setup()
        {
            _notificacionServiceMock = new Mock<INotificacionService>();
            _notificacionController = new NotificacionController(_notificacionServiceMock.Object);
        }

        [Test]
        public void CrearNotificacion_Valido_RetornaOk()
        {
            var comando = new CrearNotificacionComando("12890061", "Solicitud aprobada", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada.");
            _notificacionServiceMock.Setup(s => s.CrearNotificacion(comando));
            
            var resultado = _notificacionController.CrearNotificacion(comando);
            
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void ObtenerNotificacionesPorUsuario_ConDatos_RetornaOk()
        {
            var carnetUsuario = "12890061";
            var notificacionesEsperadas = new List<NotificacionDto>
            {
                new NotificacionDto { Id = "68535f7ddd47665ee70310b7", CarnetUsuario = "12890061", Titulo = "Solicitud aprobada", Contenido = "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada. Pue…", FechaEnvio = DateTime.Parse("2025-06-12T09:15:00.000Z") },
                new NotificacionDto { Id = "68535f7ddd47665ee70310b8", CarnetUsuario = "12890061", Titulo = "Solicitud rechazada", Contenido = "Tu solicitud de préstamo para Monitor Profesional ha sido rechazada de…", FechaEnvio = DateTime.Parse("2025-06-14T10:30:00.000Z") }
            };
            _notificacionServiceMock.Setup(s => s.ObtenerNotificacionesPorUsuario(It.Is<ObtenerNotificacionPorCarnetUsuarioConsulta>(c => c.CarnetUsuario == carnetUsuario))).Returns(notificacionesEsperadas);
            
            var resultado = _notificacionController.ObtenerNotificacionesPorUsuario(carnetUsuario);
            
            Assert.That(resultado.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(notificacionesEsperadas));
        }

        [Test]
        public void EliminarNotificacion_Valido_RetornaNoContent()
        {
            var idNotificacion = "68535f7ddd47665ee70310b7";
            _notificacionServiceMock.Setup(s => s.EliminarNotificacion(It.Is<EliminarNotificacionComando>(c => c.Id == idNotificacion)));
            
            var resultado = _notificacionController.EliminarNotificacion(idNotificacion);
            
            Assert.That(resultado, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void MarcarComoLeida_Valido_RetornaOk()
        {
            var idNotificacion = "68535f7ddd47665ee70310b7";
            var comando = new MarcarComoLeidoComando(idNotificacion);
            _notificacionServiceMock.Setup(s => s.MarcarNotificacionComoLeida(comando));
            
            var resultado = _notificacionController.MarcarComoLeida(idNotificacion);
            
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void CrearNotificacion_ServicioError_RetornaError500()
        {
            var comando = new CrearNotificacionComando("123", "Error", "Error");
            _notificacionServiceMock.Setup(s => s.CrearNotificacion(comando)).Throws(new System.Exception("Error General Servidor"));
            
            var resultado = _notificacionController.CrearNotificacion(comando);
            
            Assert.That(resultado, Is.InstanceOf<ObjectResult>());
            var objectResult = resultado as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void ObtenerNotificacionesPorUsuario_SinDatos_RetornaOkVacio()
        {
            var carnetUsuario = "usuario_sin_notificaciones";
            _notificacionServiceMock.Setup(s => s.ObtenerNotificacionesPorUsuario(It.IsAny<ObtenerNotificacionPorCarnetUsuarioConsulta>())).Returns(new List<NotificacionDto>());

            var resultado = _notificacionController.ObtenerNotificacionesPorUsuario(carnetUsuario);

            Assert.That(resultado.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado.Result as OkObjectResult;
            Assert.That((List<NotificacionDto>)okResult.Value, Is.Empty);
        }
    }
}
