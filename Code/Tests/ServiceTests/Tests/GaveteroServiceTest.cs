using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class GaveteroServiceTest : IGaveteroServiceTest
    {
        private Mock<GaveteroRepository> _gaveteroRepositoryMock;
        private GaveteroService          _gaveteroService;

        [SetUp]
        public void Setup()
        {
            _gaveteroRepositoryMock = new Mock<GaveteroRepository>();
            _gaveteroService        = new GaveteroService(_gaveteroRepositoryMock.Object);
        }

        [Test]
        public void CrearGavetero_ComandoValido_LlamaRepositorioCrear()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", 50, 40, 20);
            _gaveteroService.CrearGavetero(comando);
            _gaveteroRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearGavetero_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("", "Estándar", "Mueble-A", 50, 40, 20);
            Assert.Throws<ErrorNombreRequerido>(() => _gaveteroService.CrearGavetero(comando));
        }

        [Test]
        public void CrearGavetero_LongitudNegativa_LanzaErrorValorNegativo()
        {
            CrearGaveteroComando comando = new CrearGaveteroComando("GAV-01", "Estándar", "Mueble-A", -10, 40, 20);
            Assert.Throws<ErrorValorNegativo>(() => _gaveteroService.CrearGavetero(comando));
        }

        [Test]
        public void ObtenerTodosGaveteros_CuandoHayDatos_RetornaListaDeDtos()
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

            _gaveteroRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(gaveterosDataTable);

            List<GaveteroDto> resultado = _gaveteroService.ObtenerTodosGaveteros();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<GaveteroDto>(g => g.Id == 1 && g.Nombre == "GAV-01"));
        }

        [Test]
        public void ActualizarGavetero_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarGaveteroComando comando = new ActualizarGaveteroComando(1, "GAV-02", "Grande", "Mueble-B", 60, 50, 25);
            _gaveteroService.ActualizarGavetero(comando);
            _gaveteroRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarGavetero_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarGaveteroComando comando = new EliminarGaveteroComando(1);
            _gaveteroService.EliminarGavetero(comando);
            _gaveteroRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
