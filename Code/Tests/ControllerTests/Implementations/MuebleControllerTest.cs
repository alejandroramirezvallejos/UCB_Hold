using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class MuebleControllerTest : IMuebleControllerTest
    {
        private Mock<IMuebleService> _muebleServiceMock;
        private MuebleController _mueblesController;

        [SetUp]
        public void Setup()
        {
            _muebleServiceMock = new Mock<IMuebleService>();
            _mueblesController = new MuebleController(_muebleServiceMock.Object);
        }

        [Test]
        public void GetMuebles_ConDatos_RetornaOk()
        {
            var mueblesEsperados = new List<MuebleDto>
            {
                new MuebleDto { Id = 3, Nombre = "FERRR" },
                new MuebleDto { Id = 4, Nombre = "ferprueba" }
            };
            _muebleServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MuebleDto>>.Success(mueblesEsperados));
            var resultado = _mueblesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(mueblesEsperados.Count));
        }

        [Test]
        public void GetMuebles_SinDatos_RetornaOkVacia()
        {
            _muebleServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MuebleDto>>.Success(new List<MuebleDto>()));
            var resultado = _mueblesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetMuebles_ServicioError_RetornaBadRequest()
        {
            _muebleServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<MuebleDto>>.Error("Error servicio"));
            var resultado = _mueblesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearMueble_Valido_RetornaCreated()
        {
            var comando = new CrearMuebleComando("Armario Metálico", "Almacenamiento", 250.00, "Depósito 2", 180.0, 90.0, 45.0);
            var dto = new MuebleDto { Id = 1, Nombre = "Armario Metálico" };
            _muebleServiceMock.Setup(s => s.Crear(It.IsAny<CrearMuebleComando>())).Returns(Result<MuebleDto>.Created(dto));
            var resultado = _mueblesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearMueble_BadRequest()
        {
            yield return new object[] { new CrearMuebleComando("", "Tipo", null, "Ubicacion", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", -10, "Ubicacion", null, null, null), new ErrorValorNegativo("costo") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", 0, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", null, 0, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new CrearMuebleComando("Nombre", "Tipo", null, "Ubicacion", null, null, 0), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearMueble_BadRequest))]
        public void CrearMueble_Invalido_RetornaBadRequest(CrearMuebleComando comando, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.Crear(It.IsAny<CrearMuebleComando>())).Returns(Result<MuebleDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _mueblesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearMueble_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearMuebleComando("ferprueba", "prueba", null, "x", null, null, null);
            _muebleServiceMock.Setup(s => s.Crear(It.IsAny<CrearMuebleComando>())).Returns(Result<MuebleDto>.Conflict());
            var resultado = _mueblesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearMueble_ServicioError_RetornaError500()
        {
            var comando = new CrearMuebleComando("Error General", "Tipo", null, "Ubicacion", null, null, null);
            _muebleServiceMock.Setup(s => s.Crear(It.IsAny<CrearMuebleComando>())).Returns(Result<MuebleDto>.Error("Error General Servidor"));
            var resultado = _mueblesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarMueble_Valido_RetornaOk()
        {
            var comando = new ActualizarMuebleComando(4, "ferprueba-actualizado", "prueba-v2", 120.00, "y", 1.5, 4.2, 0.6);
            var dto = new MuebleDto { Id = 4, Nombre = "ferprueba-actualizado" };
            _muebleServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarMuebleComando>())).Returns(Result<MuebleDto>.Success(dto));
            var resultado = _mueblesController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarMueble_BadRequest()
        {
            yield return new object[] { new ActualizarMuebleComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarMuebleComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, -10, null, null, null, null), new ErrorValorNegativo("costo") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, 0, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, null, 0, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new ActualizarMuebleComando(1, "Nombre", null, null, null, null, null, 0), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarMueble_BadRequest))]
        public void ActualizarMueble_Invalido_RetornaBadRequest(ActualizarMuebleComando comando, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarMuebleComando>())).Returns(Result<MuebleDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _mueblesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarMueble_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarMuebleComando(99, "NoExiste", null, null, null, null, null, null);
            _muebleServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarMuebleComando>())).Returns(Result<MuebleDto>.NotFound());
            var resultado = _mueblesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarMueble_ServicioError_RetornaError500()
        {
            var comando = new ActualizarMuebleComando(1, "Error General", null, null, null, null, null, null);
            _muebleServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarMuebleComando>())).Returns(Result<MuebleDto>.Error("Error General Servidor"));
            var resultado = _mueblesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarMueble_Valido_RetornaNoContent()
        {
            int idValido = 3;
            var dto = new MuebleDto { Id = idValido };
            _muebleServiceMock.Setup(s => s.Eliminar(It.Is<EliminarMuebleComando>(c => c.Id == idValido))).Returns(Result<MuebleDto>.Success(dto));
            var resultado = _mueblesController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarMueble_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarMueble_BadRequest))]
        public void EliminarMueble_Invalido_RetornaBadRequest(int idMueble, System.Exception excepcionLanzada)
        {
            _muebleServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMuebleComando>())).Returns(Result<MuebleDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _mueblesController.Eliminar(idMueble);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarMueble_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _muebleServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMuebleComando>())).Returns(Result<MuebleDto>.NotFound());
            var resultado = _mueblesController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarMueble_EnUso_RetornaConflict()
        {
            int idEnUso = 5;
            _muebleServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMuebleComando>())).Returns(Result<MuebleDto>.Conflict());
            var resultado = _mueblesController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarMueble_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _muebleServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarMuebleComando>())).Returns(Result<MuebleDto>.Error("Error General Servidor"));
            var resultado = _mueblesController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
