using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EquipoServiceTest : IEquipoServiceTest
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
            CrearEquipoComando comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
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
            equiposDataTable.Columns.Add("id_grupo_equipo", typeof(int));
            equiposDataTable.Columns.Add("codigo_imt", typeof(int));
            equiposDataTable.Columns.Add("descripcion", typeof(string));
            equiposDataTable.Columns.Add("estado_equipo", typeof(string));
            equiposDataTable.Columns.Add("numero_serial", typeof(string));
            equiposDataTable.Columns.Add("ubicacion", typeof(string));
            equiposDataTable.Columns.Add("costo_referencia", typeof(double));
            equiposDataTable.Columns.Add("tiempo_max_prestamo", typeof(int));
            equiposDataTable.Columns.Add("procedencia", typeof(string));
            equiposDataTable.Columns.Add("id_gavetero", typeof(int));
            equiposDataTable.Columns.Add("estado_eliminado", typeof(bool));
            equiposDataTable.Columns.Add("fecha_ingreso_equipo", typeof(DateTime));
            equiposDataTable.Columns.Add("codigo_ucb", typeof(string));

            equiposDataTable.Rows.Add(2, 1, 33, "Impresora", "operativo", null, "Pared derecha lab", null, null, null, null, false, DateTime.Now, "JJJJJJ");
            equiposDataTable.Rows.Add(4, 3, 35, "Fuente de alimentación DC", "operativo", null, "Pared de frente lab", null, null, null, null, false, DateTime.Now, null);
            equiposDataTable.Rows.Add(7, 5, 3, "Prueba", "inoperativo", null, "No existe", 400, 1, null, 1, false, DateTime.Now, null);


            _equipoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(equiposDataTable);

            List<EquipoDto> resultado = _equipoService.ObtenerTodosEquipos();
            Assert.That(resultado, Has.Count.EqualTo(3));
            Assert.That(resultado[0].Id, Is.EqualTo(2));
            Assert.That(resultado[1].Id, Is.EqualTo(4));
            Assert.That(resultado[2].Id, Is.EqualTo(7));
        }

        [Test]
        public void ActualizarEquipo_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            _equipoService.ActualizarEquipo(comando);
            _equipoRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarEquipo_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarEquipoComando comando = new EliminarEquipoComando(5);
            _equipoService.EliminarEquipo(comando);
            _equipoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
