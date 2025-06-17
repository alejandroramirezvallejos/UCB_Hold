using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EquipoServiceTest
    {
        private Mock<EquipoRepository> _equipoRepositoryMock;
        private EquipoService          _equipoService;

        [SetUp]
        public void Setup()
        {
            _equipoRepositoryMock = new Mock<EquipoRepository>();
            _equipoService        = new EquipoService(_equipoRepositoryMock.Object);
        }

        [Test]
        public void CrearEquipo_ComandoValido_LlamaRepositorioCrear()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Laptop", "ThinkPad T480", "Lenovo", "UCB-LAP-01", "Laptop de alto rendimiento", "SN12345", "Oficina TI", "Compra directa", 1200.00, 5, "GAV-01");
            _equipoService.CrearEquipo(comando);
            _equipoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearEquipo_NombreGrupoVacio_LanzaErrorNombreRequerido()
        {
            CrearEquipoComando comando = new CrearEquipoComando("", "ThinkPad T480", "Lenovo", null, null, null, null, null, null, null, null);
            Assert.Throws<ErrorNombreRequerido>(() => _equipoService.CrearEquipo(comando));
        }

        [Test]
        public void ObtenerTodosEquipos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable equiposDataTable = new DataTable();
            equiposDataTable.Columns.Add("id_equipo", typeof(int));
            equiposDataTable.Columns.Add("nombre_grupo_equipo", typeof(string));
            equiposDataTable.Columns.Add("codigo_imt_equipo", typeof(int));
            equiposDataTable.Columns.Add("codigo_ucb_equipo", typeof(string));
            equiposDataTable.Columns.Add("descripcion_equipo", typeof(string));
            equiposDataTable.Columns.Add("numero_serial_equipo", typeof(string));
            equiposDataTable.Columns.Add("ubicacion_equipo", typeof(string));
            equiposDataTable.Columns.Add("procedencia_equipo", typeof(string));
            equiposDataTable.Columns.Add("tiempo_max_prestamo_equipo", typeof(int));
            equiposDataTable.Columns.Add("nombre_gavetero_equipo", typeof(string));
            equiposDataTable.Columns.Add("estado_equipo_equipo", typeof(string));
            equiposDataTable.Columns.Add("costo_referencia_equipo", typeof(double));
            equiposDataTable.Rows.Add(1, "Laptop", 123, "UCB-LAP-01", "desc", "SN123", "ubi", "proc", 5, "gav", "disp", 1200.0);

            _equipoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(equiposDataTable);

            List<EquipoDto> resultado = _equipoService.ObtenerTodosEquipos();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<EquipoDto>(e => e.Id == 1 && e.NombreGrupoEquipo == "Laptop"));
        }

        [Test]
        public void ActualizarEquipo_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(1, "Laptop Gaming", "UCB-LAP-02", "desc act", "SN54321", "Oficina 2", "Donación", 1500.00, 3, "GAV-02", "En mantenimiento");
            _equipoService.ActualizarEquipo(comando);
            _equipoRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarEquipo_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarEquipoComando comando = new EliminarEquipoComando(1);
            _equipoService.EliminarEquipo(comando);
            _equipoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}

