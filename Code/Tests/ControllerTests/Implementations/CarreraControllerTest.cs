using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests.Tests
{
    [TestFixture]
    public class CarreraControllerTest
    {
        private Mock<ICarreraService> _carreraServiceMock;
        private CarreraController _carrerasController;

        [SetUp]
        public void Setup()
        {
            _carreraServiceMock = new Mock<ICarreraService>();
            _carrerasController = new CarreraController(_carreraServiceMock.Object);
        }

        [Test]
        public void GetCarreras_ConDatos_RetornaOk()
        {
            var carrerasEsperadas = new List<CarreraDto>
            {
                new CarreraDto { Id = 1, Nombre = "Mecatronica" },
                new CarreraDto { Id = 2, Nombre = "Software" }
            };
            _carreraServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CarreraDto>>.Success(carrerasEsperadas));
            var resultado = _carrerasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(carrerasEsperadas.Count));
        }

        [Test]
        public void GetCarreras_SinDatos_RetornaOkVacia()
        {
            _carreraServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CarreraDto>>.Success(new List<CarreraDto>()));
            var resultado = _carrerasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetCarreras_ServicioError_RetornaBadRequest()
        {
            _carreraServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<CarreraDto>>.Error("Error servicio"));
            var resultado = _carrerasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearCarrera_Valido_RetornaCreated()
        {
            var comando = new CrearCarreraComando("Psicopedagogía");
            var dto = new CarreraDto { Id = 1, Nombre = "Psicopedagogía" };
            _carreraServiceMock.Setup(s => s.Crear(It.IsAny<CrearCarreraComando>())).Returns(Result<CarreraDto>.Created(dto));
            var resultado = _carrerasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearCarrera_BadRequest()
        {
            yield return new object[] { new CrearCarreraComando(""), new ErrorNombreRequerido() };
            yield return new object[] { new CrearCarreraComando(new string('a', 257)), new ErrorLongitudInvalida("nombre de la carrera", 256) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearCarrera_BadRequest))]
        public void CrearCarrera_Invalido_RetornaBadRequest(CrearCarreraComando comando, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.Crear(It.IsAny<CrearCarreraComando>())).Returns(Result<CarreraDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _carrerasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearCarrera_NombreExistente_RetornaConflict()
        {
            var comando = new CrearCarreraComando("Software");
            _carreraServiceMock.Setup(s => s.Crear(It.IsAny<CrearCarreraComando>())).Returns(Result<CarreraDto>.Conflict());
            var resultado = _carrerasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearCarrera_ServicioError_RetornaError500()
        {
            var comando = new CrearCarreraComando("Error General");
            _carreraServiceMock.Setup(s => s.Crear(It.IsAny<CrearCarreraComando>())).Returns(Result<CarreraDto>.Error("Error General Servidor"));
            var resultado = _carrerasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarCarrera_Valido_RetornaOk()
        {
            var comando = new ActualizarCarreraComando(5, "Ingeniería Civil");
            var dto = new CarreraDto { Id = 5, Nombre = "Ingeniería Civil" };
            _carreraServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.Success(dto));
            var resultado = _carrerasController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarCarrera_BadRequest()
        {
            yield return new object[] { new ActualizarCarreraComando(0, "Inválido"), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarCarreraComando(1, ""), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarCarreraComando(1, new string('a', 257)), new ErrorLongitudInvalida("nombre de la carrera", 256) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarCarrera_BadRequest))]
        public void ActualizarCarrera_Invalido_RetornaBadRequest(ActualizarCarreraComando comando, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _carrerasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarCarrera_NoEncontrada_RetornaNotFound()
        {
            var comando = new ActualizarCarreraComando(99, "NoExiste");
            _carreraServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.NotFound());
            var resultado = _carrerasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarCarrera_NombreExistente_RetornaConflict()
        {
            var comando = new ActualizarCarreraComando(1, "Software");
            _carreraServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.Conflict());
            var resultado = _carrerasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ActualizarCarrera_ServicioError_RetornaError500()
        {
            var comando = new ActualizarCarreraComando(1, "Error General");
            _carreraServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.Error("Error General Servidor"));
            var resultado = _carrerasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarCarrera_Valido_RetornaNoContent()
        {
            int idValido = 25;
            var dto = new CarreraDto { Id = idValido };
            _carreraServiceMock.Setup(s => s.Eliminar(It.Is<EliminarCarreraComando>(c => c.Id == idValido))).Returns(Result<CarreraDto>.Success(dto));
            var resultado = _carrerasController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarCarrera_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarCarrera_BadRequest))]
        public void EliminarCarrera_Invalido_RetornaBadRequest(int idCarrera, Exception excepcionLanzada)
        {
            _carreraServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCarreraComando>())).Returns(Result<CarreraDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _carrerasController.Eliminar(idCarrera);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarCarrera_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _carreraServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCarreraComando>())).Returns(Result<CarreraDto>.NotFound());
            var resultado = _carrerasController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarCarrera_EnUso_RetornaConflict()
        {
            int idEnUso = 23;
            _carreraServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCarreraComando>())).Returns(Result<CarreraDto>.Conflict());
            var resultado = _carrerasController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarCarrera_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _carreraServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarCarreraComando>())).Returns(Result<CarreraDto>.Error("Error General Servidor"));
            var resultado = _carrerasController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
