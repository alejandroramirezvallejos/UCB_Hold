using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MantenimientoServiceTest : IMantenimientoServiceTest
    {
        private Mock<IMantenimientoRepository> _mantenimientoRepositoryMock;
        private MantenimientoService          _mantenimientoService;

        [SetUp]
        public void Setup()
        {
            _mantenimientoRepositoryMock = new Mock<IMantenimientoRepository>();
            _mantenimientoService        = new MantenimientoService(_mantenimientoRepositoryMock.Object);
        }

        [Test]
        public void CrearMantenimiento_ComandoValido_LlamaRepositorioCrear()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "desc" });
            _mantenimientoService.CrearMantenimiento(comando);
            _mantenimientoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearMantenimiento_FechaFinAnteriorAInicio_LanzaErrorFechaInvalida()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "desc" });
            Assert.Throws<ErrorFechaInvalida>(() => _mantenimientoService.CrearMantenimiento(comando));
        }

        [Test]
        public void ObtenerTodosMantenimientos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable mantenimientosDataTable = new DataTable();
            mantenimientosDataTable.Columns.Add("id_mantenimiento", typeof(int));
            mantenimientosDataTable.Columns.Add("fecha_mantenimiento", typeof(DateTime));
            mantenimientosDataTable.Columns.Add("fecha_final_mantenimiento", typeof(DateTime));
            mantenimientosDataTable.Columns.Add("nombre_empresa_mantenimiento", typeof(string));
            mantenimientosDataTable.Columns.Add("costo_mantenimiento", typeof(double));
            mantenimientosDataTable.Columns.Add("descripcion_mantenimiento", typeof(string));
            mantenimientosDataTable.Columns.Add("codigo_imt_equipo", typeof(int));
            mantenimientosDataTable.Columns.Add("nombre_grupo_equipo", typeof(string));
            mantenimientosDataTable.Columns.Add("tipo_detalle_mantenimiento", typeof(string));
            mantenimientosDataTable.Columns.Add("descripcion_equipo", typeof(string));

            mantenimientosDataTable.Rows.Add(1, new DateTime(2025, 4, 4), new DateTime(2025, 1, 1), "Empresa Ficticia 1", DBNull.Value, DBNull.Value, 1, "Equipo 1", "Preventivo", "Desc 1");
            mantenimientosDataTable.Rows.Add(2, new DateTime(2026, 1, 1), new DateTime(2026, 2, 2), "Empresa Ficticia 1", DBNull.Value, DBNull.Value, 2, "Equipo 2", "Correctivo", "Desc 2");
            mantenimientosDataTable.Rows.Add(7, new DateTime(2027, 1, 1), new DateTime(2027, 5, 5), "Empresa Ficticia 1", 100.0, "asdadasd", 3, "Equipo 3", "Preventivo", "Desc 3");

            _mantenimientoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(mantenimientosDataTable);

            List<MantenimientoDto> resultado = _mantenimientoService.ObtenerTodosMantenimientos();
            Assert.That(resultado, Has.Count.EqualTo(3));
            Assert.That(resultado[0].Id, Is.EqualTo(1));
            Assert.That(resultado[1].Id, Is.EqualTo(2));
            Assert.That(resultado[2].Id, Is.EqualTo(7));
            Assert.That(resultado[2].Descripcion, Is.EqualTo("asdadasd"));
        }

        [Test]
        public void EliminarMantenimiento_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarMantenimientoComando comando = new EliminarMantenimientoComando(7);
            _mantenimientoService.EliminarMantenimiento(comando);
            _mantenimientoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
