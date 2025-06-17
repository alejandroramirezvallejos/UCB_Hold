using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MuebleServiceTest
    {
        private Mock<MuebleRepository> _muebleRepositoryMock;
        private MuebleService          _muebleService;

        [SetUp]
        public void Setup()
        {
            _muebleRepositoryMock = new Mock<MuebleRepository>();
            _muebleService        = new MuebleService(_muebleRepositoryMock.Object);
        }

        [Test]
        public void CrearMueble_ComandoValido_LlamaRepositorioCrear()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);
            _muebleService.CrearMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearMueble_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearMuebleComando comando = new CrearMuebleComando("", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);
            Assert.Throws<ErrorNombreRequerido>(() => _muebleService.CrearMueble(comando));
        }

        [Test]
        public void CrearMueble_CostoNegativo_LanzaErrorValorNegativo()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", -10.0, "Sala 1", 120.0, 60.0, 75.0);
            Assert.Throws<ErrorValorNegativo>(() => _muebleService.CrearMueble(comando));
        }

        [Test]
        public void ObtenerTodosMuebles_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable mueblesDataTable = new DataTable();
            mueblesDataTable.Columns.Add("id_mueble", typeof(int));
            mueblesDataTable.Columns.Add("nombre_mueble", typeof(string));
            mueblesDataTable.Columns.Add("numero_gaveteros_mueble", typeof(int));
            mueblesDataTable.Columns.Add("ubicacion_mueble", typeof(string));
            mueblesDataTable.Columns.Add("tipo_mueble", typeof(string));
            mueblesDataTable.Columns.Add("costo_mueble", typeof(double));
            mueblesDataTable.Columns.Add("longitud_mueble", typeof(double));
            mueblesDataTable.Columns.Add("profundidad_mueble", typeof(double));
            mueblesDataTable.Columns.Add("altura_mueble", typeof(double));
            mueblesDataTable.Rows.Add(1, "Escritorio", 2, "Sala 1", "Oficina", 150.00, 120.0, 60.0, 75.0);

            _muebleRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(mueblesDataTable);

            List<MuebleDto> resultado = _muebleService.ObtenerTodosMuebles();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<MuebleDto>(m => m.Id == 1 && m.Nombre == "Escritorio"));
        }

        [Test]
        public void ActualizarMueble_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(1, "Mesa de Reuniones", "Oficina", 250.00, "Sala 2", 200.0, 100.0, 75.0);
            _muebleService.ActualizarMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarMueble_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarMuebleComando comando = new EliminarMuebleComando(1);
            _muebleService.EliminarMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}

