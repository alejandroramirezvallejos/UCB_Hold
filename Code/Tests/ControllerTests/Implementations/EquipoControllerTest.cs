using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class EquipoControllerTest
    {
        private Mock<IEquipoService> _equipoServiceMock;
        private EquipoController _equiposController;

        [SetUp]
        public void Setup()
        {
            _equipoServiceMock = new Mock<IEquipoService>();
            _equiposController = new EquipoController(_equipoServiceMock.Object);
        }

        [Test]
        public void GetEquipos_ConDatos_RetornaOk()
        {
            var equiposEsperados = new List<EquipoDto>
            {
                new EquipoDto { Id = 2, NombreGrupoEquipo = "Impresora" },
                new EquipoDto { Id = 4, NombreGrupoEquipo = "Fuente de alimentación DC" }
            };
            _equipoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EquipoDto>>.Success(equiposEsperados));
            var resultado = _equiposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(equiposEsperados.Count));
        }

        [Test]
        public void GetEquipos_SinDatos_RetornaOkVacia()
        {
            _equipoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EquipoDto>>.Success(new List<EquipoDto>()));
            var resultado = _equiposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetEquipos_ServicioError_RetornaBadRequest()
        {
            _equipoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<EquipoDto>>.Error("Error servicio"));
            var resultado = _equiposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearEquipo_Valido_RetornaCreated()
        {
            var comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
            var dto = new EquipoDto { Id = 1, NombreGrupoEquipo = "Osciloscopio" };
            _equipoServiceMock.Setup(s => s.Crear(It.IsAny<CrearEquipoComando>())).Returns(Result<EquipoDto>.Created(dto));
            var resultado = _equiposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearEquipo_BadRequest()
        {
            yield return new object[] { new CrearEquipoComando("", "Modelo", "Marca", null, null, null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "", "Marca", null, null, null, null, null, null, null, null), new ErrorModeloRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "", null, null, null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "Marca", null, null, null, null, null, -100, null, null), new ErrorValorNegativo("costo de referencia") };
            yield return new object[] { new CrearEquipoComando("Laptop", "Modelo", "Marca", null, null, null, null, null, null, 0, null), new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearEquipo_BadRequest))]
        public void CrearEquipo_Invalido_RetornaBadRequest(CrearEquipoComando comando, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.Crear(It.IsAny<CrearEquipoComando>())).Returns(Result<EquipoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _equiposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearEquipo_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearEquipoComando("Impresora", "HP", "LaserJet", "JJJJJJ", null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.Crear(It.IsAny<CrearEquipoComando>())).Returns(Result<EquipoDto>.Conflict());
            var resultado = _equiposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearEquipo_ServicioError_RetornaError500()
        {
            var comando = new CrearEquipoComando("Error General", "ModeloErr", "MarcaErr", null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.Crear(It.IsAny<CrearEquipoComando>())).Returns(Result<EquipoDto>.Error("Error General Servidor"));
            var resultado = _equiposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarEquipo_Valido_RetornaOk()
        {
            var comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            var dto = new EquipoDto { Id = 7, NombreGrupoEquipo = "Prueba Actualizada" };
            _equipoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEquipoComando>())).Returns(Result<EquipoDto>.Success(dto));
            var resultado = _equiposController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarEquipo_BadRequest()
        {
            yield return new object[] { new ActualizarEquipoComando(0, null, null, null, null, null, null, null, null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarEquipoComando(1, null, null, null, null, null, null, null, null, -100, null, null, null), new ErrorValorNegativo("costo de referencia") };
            yield return new object[] { new ActualizarEquipoComando(1, null, null, null, null, null, null, null, null, null, 0, null, null), new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarEquipo_BadRequest))]
        public void ActualizarEquipo_Invalido_RetornaBadRequest(ActualizarEquipoComando comando, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEquipoComando>())).Returns(Result<EquipoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _equiposController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarEquipo_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarEquipoComando(99, "NoExiste", null, null, null, null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEquipoComando>())).Returns(Result<EquipoDto>.NotFound());
            var resultado = _equiposController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void ActualizarEquipo_ServicioError_RetornaError500()
        {
            var comando = new ActualizarEquipoComando(1, "Error General", null, null, null, null, null, null, null, null, null, null, null);
            _equipoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarEquipoComando>())).Returns(Result<EquipoDto>.Error("Error General Servidor"));
            var resultado = _equiposController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void EliminarEquipo_Valido_RetornaNoContent()
        {
            int idValido = 5;
            var dto = new EquipoDto { Id = idValido };
            _equipoServiceMock.Setup(s => s.Eliminar(It.Is<EliminarEquipoComando>(c => c.Id == idValido))).Returns(Result<EquipoDto>.Success(dto));
            var resultado = _equiposController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_EliminarEquipo_BadRequest()
        {
            yield return new object[] { 0, new ErrorIdInvalido("Id inválido") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_EliminarEquipo_BadRequest))]
        public void EliminarEquipo_Invalido_RetornaBadRequest(int idEquipo, System.Exception excepcionLanzada)
        {
            _equipoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEquipoComando>())).Returns(Result<EquipoDto>.Invalid(new ValidationError("Id", excepcionLanzada.Message)));
            var resultado = _equiposController.Eliminar(idEquipo);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void EliminarEquipo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _equipoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEquipoComando>())).Returns(Result<EquipoDto>.NotFound());
            var resultado = _equiposController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarEquipo_EnUso_RetornaConflict()
        {
            int idEnUso = 13;
            _equipoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEquipoComando>())).Returns(Result<EquipoDto>.Conflict());
            var resultado = _equiposController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void EliminarEquipo_ServicioError_RetornaError500()
        {
            int idErrorGeneral = 4;
            _equipoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarEquipoComando>())).Returns(Result<EquipoDto>.Error("Error General Servidor"));
            var resultado = _equiposController.Eliminar(idErrorGeneral);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }
    }
}
