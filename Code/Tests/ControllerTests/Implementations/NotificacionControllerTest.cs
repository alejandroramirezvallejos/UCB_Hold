using Moq;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

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
        public void CrearNotificacion_Valido_RetornaDto()
        {
            var comando = new CrearNotificacionComando("12890061", "Solicitud aprobada", "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada.");
            var dto = new NotificacionDto { Id = "abc123", CarnetUsuario = "12890061", Titulo = "Solicitud aprobada" };
            _notificacionServiceMock.Setup(s => s.Crear(It.IsAny<CrearNotificacionComando>())).Returns(Result<NotificacionDto>.Success(dto));
            var resultado = _notificacionController.Crear(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado, Is.InstanceOf<NotificacionDto>());
        }

        [Test]
        public void ObtenerNotificacionesPorUsuario_ConDatos_RetornaOk()
        {
            var carnetUsuario = "12890061";
            var notificacionesEsperadas = new List<NotificacionDto>
            {
                new NotificacionDto { Id = "68535f7ddd47665ee70310b7", CarnetUsuario = "12890061", Titulo = "Solicitud aprobada", Contenido = "Tu solicitud de préstamo para Router Inalámbrico ha sido aprobada.", FechaEnvio = DateTime.Parse("2025-06-12T09:15:00.000Z"), Leido = false },
                new NotificacionDto { Id = "68535f7ddd47665ee70310b8", CarnetUsuario = "12890061", Titulo = "Solicitud rechazada", Contenido = "Tu solicitud de préstamo para Monitor Profesional ha sido rechazada.", FechaEnvio = DateTime.Parse("2025-06-14T10:30:00.000Z"), Leido = false }
            };
            _notificacionServiceMock.Setup(s => s.ObtenerNotificacionesPorUsuario(It.Is<ObtenerNotificacionPorCarnetUsuarioConsulta>(c => c.CarnetUsuario == carnetUsuario))).Returns(notificacionesEsperadas);
            var resultado = _notificacionController.ObtenerPorUsuario(carnetUsuario);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(notificacionesEsperadas));
        }

        [Test]
        public void ObtenerNotificacionesPorUsuario_SinDatos_RetornaOkVacio()
        {
            var carnetUsuario = "usuario_sin_notificaciones";
            _notificacionServiceMock.Setup(s => s.ObtenerNotificacionesPorUsuario(It.IsAny<ObtenerNotificacionPorCarnetUsuarioConsulta>())).Returns(new List<NotificacionDto>());
            var resultado = _notificacionController.ObtenerPorUsuario(carnetUsuario);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            Assert.That((List<NotificacionDto>)okResult.Value, Is.Empty);
        }

        [Test]
        public void EliminarNotificacion_Valido_RetornaNoContent()
        {
            var idNotificacion = "68535f7ddd47665ee70310b7";
            _notificacionServiceMock.Setup(s => s.Eliminar(It.Is<EliminarNotificacionComando>(c => c.Id == idNotificacion))).Returns(Result<NotificacionDto>.Success(new NotificacionDto()));
            var resultado = _notificacionController.Eliminar(idNotificacion);
            Assert.That(resultado, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void MarcarComoLeida_Valido_RetornaOk()
        {
            var idNotificacion = "68535f7ddd47665ee70310b7";
            _notificacionServiceMock.Setup(s => s.MarcarNotificacionComoLeida(It.IsAny<MarcarComoLeidoComando>()));
            var resultado = _notificacionController.MarcarComoLeida(idNotificacion);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void TieneNoLeidas_True_RetornaOkTrue()
        {
            var carnetUsuario = "12890061";
            _notificacionServiceMock.Setup(s => s.TieneNotificacionesNoLeidas(It.Is<TieneNotificacionesNoLeidasConsulta>(c => c.CarnetUsuario == carnetUsuario))).Returns(true);
            var resultado = _notificacionController.TieneNoLeidas(carnetUsuario);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            var tieneNoLeidas = okResult.Value.GetType().GetProperty("tieneNoLeidas").GetValue(okResult.Value, null);
            Assert.That((bool)tieneNoLeidas, Is.True);
        }

        [Test]
        public void TieneNoLeidas_False_RetornaOkFalse()
        {
            var carnetUsuario = "12890061";
            _notificacionServiceMock.Setup(s => s.TieneNotificacionesNoLeidas(It.Is<TieneNotificacionesNoLeidasConsulta>(c => c.CarnetUsuario == carnetUsuario))).Returns(false);
            var resultado = _notificacionController.TieneNoLeidas(carnetUsuario);
            Assert.That(resultado, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado as OkObjectResult;
            var tieneNoLeidas = okResult.Value.GetType().GetProperty("tieneNoLeidas").GetValue(okResult.Value, null);
            Assert.That((bool)tieneNoLeidas, Is.False);
        }
    }
}
