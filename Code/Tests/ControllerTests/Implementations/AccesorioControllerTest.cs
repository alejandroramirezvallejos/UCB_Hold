using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class AccesorioControllerTest
    {
        private Mock<IAccesorioService> _accesorioServiceMock;
        private AccesorioController _accesoriosController;

        [SetUp]
        public void Setup()
        {
            _accesorioServiceMock = new Mock<IAccesorioService>();
            _accesoriosController = new AccesorioController(_accesorioServiceMock.Object);
        }

        [Test]
        public void GetAccesorios_ConDatos_RetornaOk()
        {
            var accesoriosEsperados = new List<AccesorioDto>
            {
                new AccesorioDto { Id = 2, Nombre = "cable usb", Modelo = "dasd" },
                new AccesorioDto { Id = 3, Nombre = "string", Modelo = "string" }
            };
            _accesorioServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<AccesorioDto>>.Success(accesoriosEsperados));
            var resultado = _accesoriosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(accesoriosEsperados.Count));
        }

        [Test]
        public void GetAccesorios_SinDatos_RetornaOkVacia()
        {
            _accesorioServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<AccesorioDto>>.Success(new List<AccesorioDto>()));
            var resultado = _accesoriosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetAccesorios_ServicioError_RetornaBadRequest()
        {
            _accesorioServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<AccesorioDto>>.Error("Error servicio"));
            var resultado = _accesoriosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearAccesorio_Valido_RetornaCreated()
        {
            var comando = new CrearAccesorioComando("Nuevo Cable", "NC-01", "Electrónico", 777, "Un cable nuevo", 20.00, "http://example.com/nc01.pdf");
            var dto = new AccesorioDto { Id = 1, Nombre = "Nuevo Cable" };
            _accesorioServiceMock.Setup(s => s.Crear(It.IsAny<CrearAccesorioComando>())).Returns(Result<AccesorioDto>.Created(dto));
            var resultado = _accesoriosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearAccesorio_BadRequest()
        {
            yield return new object[] { new CrearAccesorioComando("", "ModeloX", "TipoY", 1004, "desc", 10.0, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearAccesorioComando("NombreValido", null, "TipoY", 1004, "desc", 10.0, null), new ErrorModeloRequerido() };
            yield return new object[] { new CrearAccesorioComando(new string('a', 257), "ModeloX", "TipoY", 1005, "desc", 10.0, null), new ErrorLongitudInvalida("nombre del accesorio", 256) };
            yield return new object[] { new CrearAccesorioComando("NombreValido", "ModeloX", "TipoY", 0, "desc", 10.0, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new CrearAccesorioComando("NombreValido", "ModeloX", "TipoY", 1, "desc", -1.0, null), new ErrorValorNegativo("precio") };
            yield return new object[] { new CrearAccesorioComando("ReferenciaInv", "ModeloR", "TipoB", 1007, "desc", 10.0, null), new ErrorReferenciaInvalida("Referencia Inválida") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearAccesorio_BadRequest))]
        public void CrearAccesorio_Invalido_RetornaBadRequest(CrearAccesorioComando comando, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.Crear(It.IsAny<CrearAccesorioComando>())).Returns(Result<AccesorioDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _accesoriosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearAccesorio_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearAccesorioComando("cable usb", "dasd", "Electrónico", 123, "desc", 15.99, null);
            _accesorioServiceMock.Setup(s => s.Crear(It.IsAny<CrearAccesorioComando>())).Returns(Result<AccesorioDto>.Conflict());
            var resultado = _accesoriosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearAccesorio_ServicioError_RetornaError500()
        {
            var comando = new CrearAccesorioComando("Error General", "ModeloErr", "TipoErr", 9999, "desc", 0, null);
            _accesorioServiceMock.Setup(s => s.Crear(It.IsAny<CrearAccesorioComando>())).Returns(Result<AccesorioDto>.Error("Error General Servidor"));
            var resultado = _accesoriosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarAccesorio_Valido_RetornaOk()
        {
            var comando = new ActualizarAccesorioComando(2, "cable usb-c", "dasd-2", "Electrónico", 123, "cable usb actualizado", 19.99, "https://datasheet.example.com/c123-v2.pdf");
            var dto = new AccesorioDto { Id = 2, Nombre = "cable usb-c" };
            _accesorioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.Success(dto));
            var resultado = _accesoriosController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarAccesorio_BadRequest()
        {
            yield return new object[] { new ActualizarAccesorioComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarAccesorioComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarAccesorioComando(1, new string('a', 101), null, null, null, null, null, null), new ErrorLongitudInvalida("nombre del accesorio", 100) };
            yield return new object[] { new ActualizarAccesorioComando(1, "Nombre Valido", null, null, 0, null, null, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new ActualizarAccesorioComando(1, "Nombre Valido", null, null, 1, null, -1, null), new ErrorValorNegativo("precio") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarAccesorio_BadRequest))]
        public void ActualizarAccesorio_Invalido_RetornaBadRequest(ActualizarAccesorioComando comando, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _accesoriosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarAccesorio_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarAccesorioComando(99, "NoExiste", null, null, null, null, null, null);
            _accesorioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.NotFound());
            var resultado = _accesoriosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarAccesorio_RegistroExistente_RetornaConflict()
        {
            var comando = new ActualizarAccesorioComando(1, "cable usb", null, null, 1002, null, null, null);
            _accesorioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.Conflict());
            var resultado = _accesoriosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ActualizarAccesorio_ServicioError_RetornaError500()
        {
            var comando = new ActualizarAccesorioComando(1, "Error General", null, null, null, null, null, null);
            _accesorioServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.Error("Error General Servidor"));
            var resultado = _accesoriosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarAccesorio_Valido_RetornaNoContent()
        {
            int idValido = 3;
            var dto = new AccesorioDto { Id = idValido };
            _accesorioServiceMock.Setup(s => s.Eliminar(It.Is<EliminarAccesorioComando>(c => c.Id == idValido))).Returns(Result<AccesorioDto>.Success(dto));
            var resultado = _accesoriosController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarAccesorio_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarAccesorio_BadRequest))]
        public void EliminarAccesorio_Invalido_RetornaBadRequest(int idAccesorio, System.Exception excepcionLanzada)
        {
            _accesorioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarAccesorioComando>())).Returns(Result<AccesorioDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _accesoriosController.Eliminar(idAccesorio);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarAccesorio_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _accesorioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarAccesorioComando>())).Returns(Result<AccesorioDto>.NotFound());
            var resultado = _accesoriosController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarAccesorio_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _accesorioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarAccesorioComando>())).Returns(Result<AccesorioDto>.Conflict());
            var resultado = _accesoriosController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarAccesorio_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _accesorioServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarAccesorioComando>())).Returns(Result<AccesorioDto>.Error("Error General Servidor"));
            var resultado = _accesoriosController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
