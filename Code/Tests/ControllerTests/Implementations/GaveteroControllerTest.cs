using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class GaveteroControllerTest
    {
        private Mock<IGaveteroService> _gaveteroServiceMock;
        private GaveteroController _gaveterosController;

        [SetUp]
        public void Setup()
        {
            _gaveteroServiceMock = new Mock<IGaveteroService>();
            _gaveterosController = new GaveteroController(_gaveteroServiceMock.Object);
        }

        [Test]
        public void GetGaveteros_ConDatos_RetornaOk()
        {
            var gaveterosEsperados = new List<GaveteroDto>
            {
                new GaveteroDto { Id = 1, Nombre = "prueba" },
                new GaveteroDto { Id = 2, Nombre = "JJJJ" }
            };
            _gaveteroServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GaveteroDto>>.Success(gaveterosEsperados));
            var resultado = _gaveterosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(gaveterosEsperados.Count));
        }

        [Test]
        public void GetGaveteros_SinDatos_RetornaOkVacia()
        {
            _gaveteroServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GaveteroDto>>.Success(new List<GaveteroDto>()));
            var resultado = _gaveterosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetGaveteros_ServicioError_RetornaBadRequest()
        {
            _gaveteroServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GaveteroDto>>.Error("Error servicio"));
            var resultado = _gaveterosController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearGavetero_Valido_RetornaCreated()
        {
            var comando = new CrearGaveteroComando("GAV-05", "Almacenamiento", "Mueble-C", 70, 50, 30);
            var dto = new GaveteroDto { Id = 1, Nombre = "GAV-05" };
            _gaveteroServiceMock.Setup(s => s.Crear(It.IsAny<CrearGaveteroComando>())).Returns(Result<GaveteroDto>.Created(dto));
            var resultado = _gaveterosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearGavetero_BadRequest()
        {
            yield return new object[] { new CrearGaveteroComando("", "Tipo", "Mueble", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", -10, null, null), new ErrorValorNegativo("longitud") };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", null, -10, null), new ErrorValorNegativo("profundidad") };
            yield return new object[] { new CrearGaveteroComando("Nombre", "Tipo", "Mueble", null, null, -10), new ErrorValorNegativo("altura") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearGavetero_BadRequest))]
        public void CrearGavetero_Invalido_RetornaBadRequest(CrearGaveteroComando comando, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.Crear(It.IsAny<CrearGaveteroComando>())).Returns(Result<GaveteroDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _gaveterosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearGavetero_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearGaveteroComando("FER", "MAGIA", "Mueble-D", null, null, null);
            _gaveteroServiceMock.Setup(s => s.Crear(It.IsAny<CrearGaveteroComando>())).Returns(Result<GaveteroDto>.Conflict());
            var resultado = _gaveterosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearGavetero_ServicioError_RetornaError500()
        {
            var comando = new CrearGaveteroComando("Error General", "Tipo", "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.Crear(It.IsAny<CrearGaveteroComando>())).Returns(Result<GaveteroDto>.Error("Error General Servidor"));
            var resultado = _gaveterosController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarGavetero_Valido_RetornaOk()
        {
            var comando = new ActualizarGaveteroComando(3, "FER Actualizado", "MAGIA v2", "Mueble-E", 1.5, 3.2, 1.2);
            var dto = new GaveteroDto { Id = 3, Nombre = "FER Actualizado" };
            _gaveteroServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGaveteroComando>())).Returns(Result<GaveteroDto>.Success(dto));
            var resultado = _gaveterosController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarGavetero_BadRequest()
        {
            yield return new object[] { new ActualizarGaveteroComando(0, "Inválido", null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarGaveteroComando(1, "", null, "Mueble", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGaveteroComando(1, "Nombre", null, "", null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGaveteroComando(1, "Nombre", null, "Mueble", -10, null, null), new ErrorValorNegativo("longitud") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarGavetero_BadRequest))]
        public void ActualizarGavetero_Invalido_RetornaBadRequest(ActualizarGaveteroComando comando, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGaveteroComando>())).Returns(Result<GaveteroDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _gaveterosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarGavetero_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarGaveteroComando(99, "NoExiste", null, "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGaveteroComando>())).Returns(Result<GaveteroDto>.NotFound());
            var resultado = _gaveterosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarGavetero_ServicioError_RetornaError500()
        {
            var comando = new ActualizarGaveteroComando(1, "Error General", null, "Mueble", null, null, null);
            _gaveteroServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGaveteroComando>())).Returns(Result<GaveteroDto>.Error("Error General Servidor"));
            var resultado = _gaveterosController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarGavetero_Valido_RetornaNoContent()
        {
            int idValido = 1;
            var dto = new GaveteroDto { Id = idValido };
            _gaveteroServiceMock.Setup(s => s.Eliminar(It.Is<EliminarGaveteroComando>(c => c.Id == idValido))).Returns(Result<GaveteroDto>.Success(dto));
            var resultado = _gaveterosController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarGavetero_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarGavetero_BadRequest))]
        public void EliminarGavetero_Invalido_RetornaBadRequest(int idGavetero, System.Exception excepcionLanzada)
        {
            _gaveteroServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGaveteroComando>())).Returns(Result<GaveteroDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _gaveterosController.Eliminar(idGavetero);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarGavetero_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _gaveteroServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGaveteroComando>())).Returns(Result<GaveteroDto>.NotFound());
            var resultado = _gaveterosController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarGavetero_EnUso_RetornaConflict()
        {
            int idEnUso = 4;
            _gaveteroServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGaveteroComando>())).Returns(Result<GaveteroDto>.Conflict());
            var resultado = _gaveterosController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarGavetero_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _gaveteroServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGaveteroComando>())).Returns(Result<GaveteroDto>.Error("Error General Servidor"));
            var resultado = _gaveterosController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
