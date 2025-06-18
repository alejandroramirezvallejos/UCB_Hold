using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MantenimientoServiceTest : IMantenimientoServiceTest
    {
        private Mock<MantenimientoRepository> _mantenimientoRepositoryMock;
        private MantenimientoService          _mantenimientoService;

        [SetUp]
        public void Setup()
        {
            _mantenimientoRepositoryMock = new Mock<MantenimientoRepository>();
            _mantenimientoService        = new MantenimientoService(_mantenimientoRepositoryMock.Object);
        }

        [Test]
        public void CrearMantenimiento_ComandoValido_LlamaRepositorioCrear()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "Equipo 1" });
            _mantenimientoService.CrearMantenimiento(comando);
            _mantenimientoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearMantenimiento_FechaFinAnteriorAInicio_LanzaErrorFechaInvalida()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "Equipo 1" });
            Assert.Throws<ErrorFechaInvalida>(() => _mantenimientoService.CrearMantenimiento(comando));
        }

        [Test]
        public void ObtenerTodosMantenimientos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable mantenimientosDataTable = new DataTable();
            mantenimientosDataTable.Columns.Add("id_mantenimiento", typeof(int));
            mantenimientosDataTable.Columns.Add("descripcion", typeof(string));
            mantenimientosDataTable.Columns.Add("costo", typeof(double));
            mantenimientosDataTable.Columns.Add("fecha_mantenimiento", typeof(DateTime));
            mantenimientosDataTable.Columns.Add("id_empresa", typeof(int));
            mantenimientosDataTable.Columns.Add("estado_eliminado", typeof(bool));
            mantenimientosDataTable.Columns.Add("fecha_final_mantenimiento", typeof(DateTime));
            mantenimientosDataTable.Columns.Add("nombre_empresa_mantenimiento", typeof(string));

            mantenimientosDataTable.Rows.Add(1, DBNull.Value, DBNull.Value, new DateTime(2025, 4, 4), 1, false, new DateTime(2025, 1, 1), "Empresa Ficticia 1");
            mantenimientosDataTable.Rows.Add(2, DBNull.Value, DBNull.Value, new DateTime(2026, 1, 1), 1, false, new DateTime(2026, 2, 2), "Empresa Ficticia 1");
            mantenimientosDataTable.Rows.Add(7, "asdadasd", 100.0, new DateTime(2027, 1, 1), 1, false, new DateTime(2027, 5, 5), "Empresa Ficticia 1");

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
