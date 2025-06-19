using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EquipoServiceTest : IEquipoServiceTest
    {
        private Mock<IEquipoRepository> _equipoRepositoryMock;
        private EquipoService          _equipoService;

        [SetUp]
        public void Setup()
        {
            _equipoRepositoryMock = new Mock<IEquipoRepository>();
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
            equiposDataTable.Columns.AddRange(new[]
            {
                new DataColumn("id_equipo", typeof(int)),
                new DataColumn("nombre_grupo_equipo", typeof(string)),
                new DataColumn("modelo_equipo", typeof(string)),
                new DataColumn("marca_equipo", typeof(string)),
                new DataColumn("codigo_imt_equipo", typeof(int)),
                new DataColumn("codigo_ucb_equipo", typeof(string)),
                new DataColumn("descripcion_equipo", typeof(string)),
                new DataColumn("numero_serial_equipo", typeof(string)),
                new DataColumn("ubicacion_equipo", typeof(string)),
                new DataColumn("procedencia_equipo", typeof(string)),
                new DataColumn("tiempo_max_prestamo_equipo", typeof(int)),
                new DataColumn("nombre_gavetero_equipo", typeof(string)),
                new DataColumn("estado_equipo_equipo", typeof(string)),
                new DataColumn("costo_referencia_equipo", typeof(double))
            });

            equiposDataTable.Rows.Add(2, "Impresoras", "LaserJet Pro", "HP", 33, "JJJJJJ", "Impresora", null, "Pared derecha lab", null, null, null, "operativo", null);
            equiposDataTable.Rows.Add(4, "Fuentes", "DC Power Supply", "Rigol", 35, null, "Fuente de alimentación DC", null, "Pared de frente lab", null, null, null, "operativo", null);
            equiposDataTable.Rows.Add(7, "Pruebas", "Tester", "Fluke", 3, null, "Prueba", null, "No existe", null, 1, "GAV-01", "inoperativo", 400);


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
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
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
