using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class EmpresaMantenimientoControllerTest : IEmpresaMantenimientoControllerTest
    {
        private Mock<IEmpresaMantenimientoService> _empresaServiceMock;
        private EmpresaMantenimientoController _empresasController;

        [SetUp]
        public void Setup()
        {
            _empresaServiceMock = new Mock<IEmpresaMantenimientoService>();
            _empresasController = new EmpresaMantenimientoController(_empresaServiceMock.Object);
        }

        [Test]
        public void GetEmpresas_ConDatos_RetornaOk()
        {
            var empresasEsperadas = new List<EmpresaMantenimientoDto>
            {
                new EmpresaMantenimientoDto { Id = 1, NombreEmpresa = "JJJ" },
                new EmpresaMantenimientoDto { Id = 2, NombreEmpresa = "PRUEBA" }
            };
            _empresaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EmpresaMantenimientoDto>>.Success(empresasEsperadas));
            var resultado = _empresasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(empresasEsperadas.Count));
        }

        [Test]
        public void GetEmpresas_SinDatos_RetornaOkVacia()
        {
            _empresaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EmpresaMantenimientoDto>>.Success(new List<EmpresaMantenimientoDto>()));
            var resultado = _empresasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetEmpresas_ServicioError_RetornaBadRequest()
        {
            _empresaServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EmpresaMantenimientoDto>>.Error("Error servicio"));
            var resultado = _empresasController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearEmpresa_Valida_RetornaCreated()
        {
            var comando = new CrearEmpresaMantenimientoComando("Mantenimiento Global", "Carlos", "Ruiz", "55555555", "Av. Principal 456", "987654321");
            var dto = new EmpresaMantenimientoDto { Id = 1, NombreEmpresa = "Mantenimiento Global" };
            _empresaServiceMock.Setup(s => s.Crear(It.IsAny<CrearEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Created(dto));
            var resultado = _empresasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearEmpresa_BadRequest()
        {
            yield return new object[] { new CrearEmpresaMantenimientoComando("", null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEmpresaMantenimientoComando(new string('a', 256), null, null, null, null, null), new ErrorLongitudInvalida("nombre", 255) };
            yield return new object[] { new CrearEmpresaMantenimientoComando("Empresa Valida", null, null, new string('a', 21), null, null), new ErrorLongitudInvalida("telefono", 20) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearEmpresa_BadRequest))]
        public void CrearEmpresa_Invalida_RetornaBadRequest(CrearEmpresaMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.Crear(It.IsAny<CrearEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _empresasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearEmpresa_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearEmpresaMantenimientoComando("PRUEBA", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.Crear(It.IsAny<CrearEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Conflict());
            var resultado = _empresasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearEmpresa_ServicioError_RetornaError500()
        {
            var comando = new CrearEmpresaMantenimientoComando("Error General", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.Crear(It.IsAny<CrearEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Error("Error General Servidor"));
            var resultado = _empresasController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarEmpresa_Valida_RetornaOk()
        {
            var comando = new ActualizarEmpresaMantenimientoComando(1, "JJJ Actualizado", "string", "string", "string", "string", "string");
            var dto = new EmpresaMantenimientoDto { Id = 1, NombreEmpresa = "JJJ Actualizado" };
            _empresaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Success(dto));
            var resultado = _empresasController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarEmpresa_BadRequest()
        {
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(0, "Inválido", null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "", null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, new string('a', 101), null, null, null, null, null), new ErrorLongitudInvalida("nombre", 100) };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "Empresa Valida", null, null, new string('a', 21), null, null), new ErrorLongitudInvalida("telefono", 20) };
            yield return new object[] { new ActualizarEmpresaMantenimientoComando(1, "Empresa Valida", null, null, null, null, new string('a', 21)), new ErrorLongitudInvalida("nit", 20) };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarEmpresa_BadRequest))]
        public void ActualizarEmpresa_Invalida_RetornaBadRequest(ActualizarEmpresaMantenimientoComando comando, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _empresasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarEmpresa_NoEncontrada_RetornaNotFound()
        {
            var comando = new ActualizarEmpresaMantenimientoComando(99, "NoExiste", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.NotFound());
            var resultado = _empresasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarEmpresa_RegistroExistente_RetornaConflict()
        {
            var comando = new ActualizarEmpresaMantenimientoComando(3, "PRUEBA", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Conflict());
            var resultado = _empresasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void ActualizarEmpresa_ServicioError_RetornaError500()
        {
            var comando = new ActualizarEmpresaMantenimientoComando(1, "Error General", null, null, null, null, null);
            _empresaServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Error("Error General Servidor"));
            var resultado = _empresasController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarEmpresa_Valida_RetornaNoContent()
        {
            int idValido = 6;
            var dto = new EmpresaMantenimientoDto { Id = idValido };
            _empresaServiceMock.Setup(s => s.Eliminar(It.Is<EliminarEmpresaMantenimientoComando>(c => c.Id == idValido))).Returns(Result<EmpresaMantenimientoDto>.Success(dto));
            var resultado = _empresasController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarEmpresa_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarEmpresa_BadRequest))]
        public void EliminarEmpresa_Invalida_RetornaBadRequest(int idEmpresa, System.Exception excepcionLanzada)
        {
            _empresaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _empresasController.Eliminar(idEmpresa);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarEmpresa_NoEncontrada_RetornaNotFound()
        {
            int idNoExistente = 99;
            _empresaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.NotFound());
            var resultado = _empresasController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarEmpresa_EnUso_RetornaConflict()
        {
            int idEnUso = 2;
            _empresaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Conflict());
            var resultado = _empresasController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarEmpresa_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _empresaServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Error("Error General Servidor"));
            var resultado = _empresasController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
