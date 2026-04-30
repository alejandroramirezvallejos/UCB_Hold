using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MantenimientoControllerTest
    {
        private Mock<IMantenimientoService> _mantenimientoServiceMock;
        private MantenimientoController _mantenimientosController;

        [SetUp]
        public void Setup()
        {
            _mantenimientoServiceMock = new Mock<IMantenimientoService>();
            _mantenimientosController = new MantenimientoController(_mantenimientoServiceMock.Object);
        }

        [Test]
        public void GetMantenimientos_ConDatos_RetornaOk()
        {
            var mantenimientosEsperados = new List<MantenimientoDto>
            {
                new MantenimientoDto { Id = 1, NombreEmpresaMantenimiento = "Empresa Ficticia 1" },
                new MantenimientoDto { Id = 2, NombreEmpresaMantenimiento = "Empresa Ficticia 1" }
            };
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MantenimientoDto>>.Success(mantenimientosEsperados));
            var resultado = _mantenimientosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(mantenimientosEsperados.Count));
        }

        [Test]
        public void GetMantenimientos_SinDatos_RetornaOkVacia()
        {
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MantenimientoDto>>.Success(new List<MantenimientoDto>()));
            var resultado = _mantenimientosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetMantenimientos_ServicioError_RetornaBadRequest()
        {
            _mantenimientoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MantenimientoDto>>.Error("Error servicio"));
            var resultado = _mantenimientosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearMantenimiento_Valido_RetornaCreated()
        {
            var comando = new CrearMantenimientoComando(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 10), "Empresa Nueva", 300.00, "Mantenimiento de servidor", new int[] { 8 }, new string[] { "Preventivo" }, new string[] { "Servidor Rack" });
            var dto = new MantenimientoDto { Id = 1, NombreEmpresaMantenimiento = "Empresa Nueva" };
            _mantenimientoServiceMock.Setup(s => s.Crear(It.IsAny<CrearMantenimientoComando>())).Returns(Result<MantenimientoDto>.Created(dto));
            var resultado = _mantenimientosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearMantenimiento_BadRequest()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorNombreRequerido() };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(-1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorFechaInvalida() };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", null, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, null, new string[] { "Equipo" }), new ErrorReferenciaInvalida("tipos de mantenimiento") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("tipos de mantenimiento") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1, 2 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorReferenciaInvalida("equipos y tipos") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Desc1", "Desc2" }), new ErrorReferenciaInvalida("descripciones") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", 100.50, "Desc", new int[] { 0 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new CrearMantenimientoComando(today, today.AddDays(1), "Empresa", -100, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" }), new ErrorValorNegativo("costo") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearMantenimiento_BadRequest))]
        public void CrearMantenimiento_Invalido_RetornaBadRequest(CrearMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _mantenimientoServiceMock.Setup(s => s.Crear(It.IsAny<CrearMantenimientoComando>())).Returns(Result<MantenimientoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _mantenimientosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearMantenimiento_ServicioError_RetornaError500()
        {
            var comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Error General", 100.50, "Desc", new int[] { 1 }, new string[] { "Tipo" }, new string[] { "Equipo" });
            _mantenimientoServiceMock.Setup(s => s.Crear(It.IsAny<CrearMantenimientoComando>())).Returns(Result<MantenimientoDto>.Error("Error General Servidor"));
            var resultado = _mantenimientosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarMantenimiento_Valido_RetornaNoContent()
        {
            int idValido = 7;
            var dto = new MantenimientoDto { Id = idValido };
            _mantenimientoServiceMock.Setup(s => s.Eliminar(It.Is<EliminarMantenimientoComando>(c => c.Id == idValido))).Returns(Result<MantenimientoDto>.Success(dto));
            var resultado = _mantenimientosController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void EliminarMantenimiento_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _mantenimientoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMantenimientoComando>())).Returns(Result<MantenimientoDto>.NotFound());
            var resultado = _mantenimientosController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarMantenimiento_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _mantenimientoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMantenimientoComando>())).Returns(Result<MantenimientoDto>.Conflict());
            var resultado = _mantenimientosController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarMantenimiento_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _mantenimientoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMantenimientoComando>())).Returns(Result<MantenimientoDto>.Error("Error General Servidor"));
            var resultado = _mantenimientosController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarMantenimiento_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarMantenimiento_BadRequest))]
        public void EliminarMantenimiento_Invalido_RetornaBadRequest(int idMantenimiento, System.Exception excepcionLanzada)
        {
            _mantenimientoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMantenimientoComando>())).Returns(Result<MantenimientoDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _mantenimientosController.Eliminar(idMantenimiento);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }
    }
}
