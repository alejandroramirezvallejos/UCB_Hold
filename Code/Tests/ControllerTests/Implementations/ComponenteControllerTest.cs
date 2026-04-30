using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class ComponenteControllerTest : IComponenteControllerTest
    {
        private Mock<IComponenteService> _componenteServiceMock;
        private ComponenteController _componentesController;

        [SetUp]
        public void Setup()
        {
            _componenteServiceMock = new Mock<IComponenteService>();
            _componentesController = new ComponenteController(_componenteServiceMock.Object);
        }

        [Test]
        public void GetComponentes_ConDatos_RetornaOk()
        {
            var componentesEsperados = new List<ComponenteDto>
            {
                new ComponenteDto { Id = 1, Nombre = "prueba componente", Modelo = "prueba" },
                new ComponenteDto { Id = 3, Nombre = "PRE", Modelo = "MODULAR" }
            };
            _componenteServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<ComponenteDto>>.Success(componentesEsperados));
            var resultado = _componentesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(componentesEsperados.Count));
        }

        [Test]
        public void GetComponentes_SinDatos_RetornaOkVacia()
        {
            _componenteServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<ComponenteDto>>.Success(new List<ComponenteDto>()));
            var resultado = _componentesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetComponentes_ServicioError_RetornaBadRequest()
        {
            _componenteServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<ComponenteDto>>.Error("Error servicio"));
            var resultado = _componentesController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearComponente_Valido_RetornaCreated()
        {
            var comando = new CrearComponenteComando("Nuevo Componente", "NC-01", "Tipo Nuevo", 5, "Desc Nuevo", 150.00, "http://example.com/nc01.pdf");
            var dto = new ComponenteDto { Id = 1, Nombre = "Nuevo Componente" };
            _componenteServiceMock.Setup(s => s.Crear(It.IsAny<CrearComponenteComando>())).Returns(Result<ComponenteDto>.Created(dto));
            var resultado = _componentesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearComponente_BadRequest()
        {
            yield return new object[] { new CrearComponenteComando("", "SN1", "Tipo", 1, "desc", 10, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearComponenteComando(new string('a', 256), "SN2", "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("nombre", 255) };
            yield return new object[] { new CrearComponenteComando("CPU", "", "Tipo", 1, "desc", 10, null), new ErrorModeloRequerido() };
            yield return new object[] { new CrearComponenteComando("CPU", new string('a', 256), "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("modelo", 255) };
            yield return new object[] { new CrearComponenteComando("CPU", "SN3", "Tipo", 0, "desc", 10, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new CrearComponenteComando("CPU", "SN4", "Tipo", 1, "desc", -10, null), new ErrorValorNegativo("precio de referencia") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearComponente_BadRequest))]
        public void CrearComponente_Invalido_RetornaBadRequest(CrearComponenteComando comando, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.Crear(It.IsAny<CrearComponenteComando>())).Returns(Result<ComponenteDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _componentesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearComponente_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearComponenteComando("prueba componente", "prueba", "jjjj", 7, "desc", 0, null);
            _componenteServiceMock.Setup(s => s.Crear(It.IsAny<CrearComponenteComando>())).Returns(Result<ComponenteDto>.Conflict());
            var resultado = _componentesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearComponente_ServicioError_RetornaError500()
        {
            var comando = new CrearComponenteComando("Error General", "ERR001", "Error", 9, "desc", 0, null);
            _componenteServiceMock.Setup(s => s.Crear(It.IsAny<CrearComponenteComando>())).Returns(Result<ComponenteDto>.Error("Error General Servidor"));
            var resultado = _componentesController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarComponente_Valido_RetornaOk()
        {
            var comando = new ActualizarComponenteComando(1, "prueba componente actualizada", "prueba-v2", "jjjj", 7, "desc actualizada", 10.00, "http://example.com/updated.pdf");
            var dto = new ComponenteDto { Id = 1, Nombre = "prueba componente actualizada" };
            _componenteServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.Success(dto));
            var resultado = _componentesController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarComponente_BadRequest()
        {
            yield return new object[] { new ActualizarComponenteComando(0, "Inválido", "modelo", "tipo", 1, null, 0, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarComponenteComando(1, "", "SN1", "Tipo", 1, "desc", 10, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarComponenteComando(1, new string('a', 101), "SN2", "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new ActualizarComponenteComando(1, "CPU", "", "Tipo", 1, "desc", 10, null), new ErrorModeloRequerido() };
            yield return new object[] { new ActualizarComponenteComando(1, "CPU", new string('a', 51), "Tipo", 1, "desc", 10, null), new ErrorLongitudInvalida("modelo", 50) };
            yield return new object[] { new ActualizarComponenteComando(1, "CPU", "SN3", "Tipo", 0, "desc", 10, null), new ErrorCodigoImtRequerido() };
            yield return new object[] { new ActualizarComponenteComando(1, "CPU", "SN4", "Tipo", 1, "desc", -10, null), new ErrorValorNegativo("precio de referencia") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarComponente_BadRequest))]
        public void ActualizarComponente_Invalido_RetornaBadRequest(ActualizarComponenteComando comando, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _componentesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarComponente_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarComponenteComando(99, "NoExiste", "NE001", null, 1, null, 0, null);
            _componenteServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.NotFound());
            var resultado = _componentesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarComponente_RegistroExistente_RetornaConflict()
        {
            var comando = new ActualizarComponenteComando(3, "prueba componente", "MODULAR", "PRE", 7, "desc", 0, null);
            _componenteServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.Conflict());
            var resultado = _componentesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ActualizarComponente_ServicioError_RetornaError500()
        {
            var comando = new ActualizarComponenteComando(1, "Error General", "ERR002", null, 0, null, 0, null);
            _componenteServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.Error("Error General Servidor"));
            var resultado = _componentesController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarComponente_Valido_RetornaNoContent()
        {
            int idValido = 4;
            var dto = new ComponenteDto { Id = idValido };
            _componenteServiceMock.Setup(s => s.Eliminar(It.Is<EliminarComponenteComando>(c => c.Id == idValido))).Returns(Result<ComponenteDto>.Success(dto));
            var resultado = _componentesController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarComponente_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarComponente_BadRequest))]
        public void EliminarComponente_Invalido_RetornaBadRequest(int idComponente, System.Exception excepcionLanzada)
        {
            _componenteServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComponenteComando>())).Returns(Result<ComponenteDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _componentesController.Eliminar(idComponente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarComponente_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _componenteServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComponenteComando>())).Returns(Result<ComponenteDto>.NotFound());
            var resultado = _componentesController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarComponente_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _componenteServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComponenteComando>())).Returns(Result<ComponenteDto>.Conflict());
            var resultado = _componentesController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarComponente_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _componenteServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarComponenteComando>())).Returns(Result<ComponenteDto>.Error("Error General Servidor"));
            var resultado = _componentesController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
