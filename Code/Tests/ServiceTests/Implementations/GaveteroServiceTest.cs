using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class GaveteroServiceTest
    {
        private Mock<IGaveteroRepository> _gaveteroRepositoryMock;
        private Mock<IMuebleRepository> _muebleRepositoryMock;
        private GaveteroService _gaveteroService;

        [SetUp]
        public void Setup()
        {
            _gaveteroRepositoryMock = new Mock<IGaveteroRepository>();
            _muebleRepositoryMock = new Mock<IMuebleRepository>();
            _gaveteroService = new GaveteroService(_gaveteroRepositoryMock.Object, _muebleRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", 50, 40, 20);
            _gaveteroRepositoryMock.Setup(r => r.ObtenerMuebleIdPorNombre(It.IsAny<string>())).Returns(1);
            _gaveteroRepositoryMock.Setup(r => r.ExisteActivoPorNombre(It.IsAny<string>())).Returns(false);
            _gaveteroRepositoryMock.Setup(r => r.Crear(It.IsAny<int>(), It.IsAny<CrearGaveteroComando>()));

            var resultado = _gaveteroService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _gaveteroRepositoryMock.Verify(r => r.Crear(It.IsAny<int>(), comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("", "Estándar", "Mueble-A", 50, 40, 20);

            var resultado = _gaveteroService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_LongitudNegativa_RetornaInvalid()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", -10, 40, 20);

            var resultado = _gaveteroService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable gaveterosDataTable = new DataTable();
            gaveterosDataTable.Columns.Add("id_gavetero", typeof(int));
            gaveterosDataTable.Columns.Add("nombre_gavetero", typeof(string));
            gaveterosDataTable.Columns.Add("tipo_gavetero", typeof(string));
            gaveterosDataTable.Columns.Add("nombre_mueble", typeof(string));
            gaveterosDataTable.Columns.Add("longitud_gavetero", typeof(double));
            gaveterosDataTable.Columns.Add("profundidad_gavetero", typeof(double));
            gaveterosDataTable.Columns.Add("altura_gavetero", typeof(double));
            gaveterosDataTable.Rows.Add(1, "GAV-01", "Estándar", "Mueble-A", 50, 40, 20);

            _gaveteroRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(gaveterosDataTable));

            var resultado = _gaveteroService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(1));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("GAV-01"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(1, "GAV-02", "Grande", "Mueble-B", 60, 50, 25);
            _gaveteroRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _gaveteroRepositoryMock.Setup(r => r.ObtenerMuebleIdPorNombre(It.IsAny<string>())).Returns(2);
            _gaveteroRepositoryMock.Setup(r => r.ObtenerMuebleIdPorGaveteroId(It.IsAny<int>())).Returns(1);
            _gaveteroRepositoryMock.Setup(r => r.Actualizar(It.IsAny<int?>(), It.IsAny<ActualizarGaveteroComando>()));

            var resultado = _gaveteroService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _gaveteroRepositoryMock.Verify(r => r.Actualizar(It.IsAny<int?>(), comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(0, "GAV-02", "Grande", "Mueble-B", 60, 50, 25);

            var resultado = _gaveteroService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarGaveteroComando comando = new EliminarGaveteroComando(1);
            _gaveteroRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _gaveteroRepositoryMock.Setup(r => r.ObtenerMuebleIdPorGaveteroId(It.IsAny<int>())).Returns(1);
            _gaveteroRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarGaveteroComando>()));

            var resultado = _gaveteroService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _gaveteroRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarGaveteroComando comando = new EliminarGaveteroComando(0);

            var resultado = _gaveteroService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
