using Moq;
using Ardalis.Result;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ControllerTests
{
    [TestFixture]
    public class GrupoEquipoControllerTest
    {
        private Mock<IGrupoEquipoService> _grupoServiceMock;
        private GrupoEquipoController _gruposController;

        [SetUp]
        public void Setup()
        {
            _grupoServiceMock = new Mock<IGrupoEquipoService>();
            _gruposController = new GrupoEquipoController(_grupoServiceMock.Object);
        }

        [Test]
        public void GetGruposEquipos_ConDatos_RetornaOk()
        {
            var gruposEsperados = new List<GrupoEquipoDto>
            {
                new GrupoEquipoDto { Id = 1, Nombre = "Impresora" },
                new GrupoEquipoDto { Id = 2, Nombre = "Soldamatics" }
            };
            _grupoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GrupoEquipoDto>>.Success(gruposEsperados));
            var resultado = _gruposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(gruposEsperados.Count));
        }

        [Test]
        public void GetGruposEquipos_SinDatos_RetornaOkVacia()
        {
            _grupoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GrupoEquipoDto>>.Success(new List<GrupoEquipoDto>()));
            var resultado = _gruposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Is.Empty);
        }

        [Test]
        public void GetGruposEquipos_ServicioError_RetornaBadRequest()
        {
            _grupoServiceMock.Setup(s => s.ObtenerTodos()).Returns(Result<List<GrupoEquipoDto>>.Error("Error servicio"));
            var resultado = _gruposController.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void CrearGrupoEquipo_Valido_RetornaCreated()
        {
            var comando = new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png");
            var dto = new GrupoEquipoDto { Id = 1, Nombre = "Estación de Soldadura" };
            _grupoServiceMock.Setup(s => s.Crear(It.IsAny<CrearGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Created(dto));
            var resultado = _gruposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Created));
        }

        private static IEnumerable<object[]> FuenteCasos_CrearGrupoEquipo_BadRequest()
        {
            yield return new object[] { new CrearGrupoEquipoComando("", "Modelo", "Marca", "Desc", "Cat", null, "img"), new ErrorNombreRequerido() };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "", "Marca", "Desc", "Cat", null, "img"), new ErrorModeloRequerido() };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "", "Desc", "Cat", null, "img"), new ErrorCampoRequerido("marca") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "", "Cat", null, "img"), new ErrorCampoRequerido("descripcion") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "Desc", "", null, "img"), new ErrorCampoRequerido("categoria") };
            yield return new object[] { new CrearGrupoEquipoComando("Nombre", "Modelo", "Marca", "Desc", "Cat", null, ""), new ErrorCampoRequerido("url de imagen") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_CrearGrupoEquipo_BadRequest))]
        public void CrearGrupoEquipo_Invalido_RetornaBadRequest(CrearGrupoEquipoComando comando, System.Exception excepcionLanzada)
        {
            _grupoServiceMock.Setup(s => s.Crear(It.IsAny<CrearGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _gruposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void CrearGrupoEquipo_RegistroExistente_RetornaConflict()
        {
            var comando = new CrearGrupoEquipoComando("Impresora", "prueba", "prueba", "prueba", "Impresoras", "string", "https://th.bing.com/th/id/OIP.u6bg7Q6XQdd5ZCfumbYt9AHaD4?cb=iwp1&rs=1&pid=ImgDetMain");
            _grupoServiceMock.Setup(s => s.Crear(It.IsAny<CrearGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Conflict());
            var resultado = _gruposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }

        [Test]
        public void CrearGrupoEquipo_ServicioError_RetornaError500()
        {
            var comando = new CrearGrupoEquipoComando("Error", "Error", "Error", "Error", "Error", null, "Error");
            _grupoServiceMock.Setup(s => s.Crear(It.IsAny<CrearGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Error("Error General Servidor"));
            var resultado = _gruposController.Crear(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Error));
        }

        [Test]
        public void ActualizarGrupoEquipo_Valido_RetornaOk()
        {
            var comando = new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https::prueba-v2", "img_act");
            var dto = new GrupoEquipoDto { Id = 5, Nombre = "prueba actualizada" };
            _grupoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Success(dto));
            var resultado = _gruposController.Actualizar(comando);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        private static IEnumerable<object[]> FuenteCasos_ActualizarGrupoEquipo_BadRequest()
        {
            yield return new object[] { new ActualizarGrupoEquipoComando(0, "Inválido", null, null, null, null, null, null), new ErrorIdInvalido("Id inválido") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "", null, null, null, null, null, null), new ErrorNombreRequerido() };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "", null, null, null, null, null), new ErrorModeloRequerido() };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "", null, null, null, null), new ErrorCampoRequerido("marca") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "", null, null, null), new ErrorCampoRequerido("descripcion") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "Desc", "", null, null), new ErrorCampoRequerido("categoria") };
            yield return new object[] { new ActualizarGrupoEquipoComando(1, "Nombre", "Modelo", "Marca", "Desc", "Cat", null, ""), new ErrorCampoRequerido("url de imagen") };
        }

        [Test]
        [TestCaseSource(nameof(FuenteCasos_ActualizarGrupoEquipo_BadRequest))]
        public void ActualizarGrupoEquipo_Invalido_RetornaBadRequest(ActualizarGrupoEquipoComando comando, System.Exception excepcionLanzada)
        {
            _grupoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Invalid(new ValidationError("campo", excepcionLanzada.Message)));
            var resultado = _gruposController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Invalid));
        }

        [Test]
        public void ActualizarGrupoEquipo_NoEncontrado_RetornaNotFound()
        {
            var comando = new ActualizarGrupoEquipoComando(99, "NoExiste", null, null, null, null, null, null);
            _grupoServiceMock.Setup(s => s.Actualizar(It.IsAny<ActualizarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.NotFound());
            var resultado = _gruposController.Actualizar(comando);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarGrupoEquipo_Valido_RetornaNoContent()
        {
            int idValido = 16;
            var dto = new GrupoEquipoDto { Id = idValido };
            _grupoServiceMock.Setup(s => s.Eliminar(It.Is<EliminarGrupoEquipoComando>(c => c.Id == idValido))).Returns(Result<GrupoEquipoDto>.Success(dto));
            var resultado = _gruposController.Eliminar(idValido);
            Assert.That(resultado.IsSuccess, Is.True);
        }

        [Test]
        public void EliminarGrupoEquipo_NoEncontrado_RetornaNotFound()
        {
            int idNoExistente = 99;
            _grupoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.NotFound());
            var resultado = _gruposController.Eliminar(idNoExistente);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.NotFound));
        }

        [Test]
        public void EliminarGrupoEquipo_EnUso_RetornaConflict()
        {
            int idEnUso = 1;
            _grupoServiceMock.Setup(s => s.Eliminar(It.IsAny<EliminarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Conflict());
            var resultado = _gruposController.Eliminar(idEnUso);
            Assert.That(resultado.Status, Is.EqualTo(ResultStatus.Conflict));
        }
    }
}
