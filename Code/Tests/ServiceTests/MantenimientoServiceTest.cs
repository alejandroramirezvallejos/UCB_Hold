using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MantenimientoServiceTest
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
        public void CrearMantenimiento_FechaFinAnteriorAInicio_LanzaErrorReferenciaInvalida()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "Equipo 1" });
            Assert.Throws<ErrorReferenciaInvalida>(() => _mantenimientoService.CrearMantenimiento(comando));
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
            mantenimientosDataTable.Rows.Add(1, DateTime.Now, DateTime.Now.AddDays(1), "Empresa A", 100.50, "Desc", 123, "Laptop", "Preventivo", "Equipo 1");

            _mantenimientoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(mantenimientosDataTable);

            List<MantenimientoDto> resultado = _mantenimientoService.ObtenerTodosMantenimientos();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<MantenimientoDto>(m => m.Id == 1 && m.NombreEmpresaMantenimiento == "Empresa A"));
        }

        [Test]
        public void EliminarMantenimiento_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarMantenimientoComando comando = new EliminarMantenimientoComando(1);
            _mantenimientoService.EliminarMantenimiento(comando);
            _mantenimientoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}

