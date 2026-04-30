using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MantenimientoServiceTest
    {
        private Mock<IMantenimientoRepository> _mantenimientoRepositoryMock;
        private MantenimientoService _mantenimientoService;

        [SetUp]
        public void Setup()
        {
            _mantenimientoRepositoryMock = new Mock<IMantenimientoRepository>();
            _mantenimientoService = new MantenimientoService(_mantenimientoRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "desc" });
            _mantenimientoRepositoryMock.Setup(r => r.ObtenerEmpresaIdPorNombre(It.IsAny<string>())).Returns(1);
            _mantenimientoRepositoryMock.Setup(r => r.ObtenerEquipoIdPorCodigoImt(It.IsAny<int>())).Returns(1);
            _mantenimientoRepositoryMock.Setup(r => r.CrearMantenimiento(It.IsAny<int>(), It.IsAny<CrearMantenimientoComando>())).Returns(1);
            _mantenimientoRepositoryMock.Setup(r => r.CrearDetalleMantenimiento(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

            var resultado = _mantenimientoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _mantenimientoRepositoryMock.Verify(r => r.CrearMantenimiento(It.IsAny<int>(), comando), Times.Once);
        }

        [Test]
        public void Crear_FechaFinAnteriorAInicio_RetornaInvalid()
        {
            CrearMantenimientoComando comando = new CrearMantenimientoComando(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "Empresa A", 100.50, "Descripción", new int[] { 1 }, new string[] { "Preventivo" }, new string[] { "desc" });

            var resultado = _mantenimientoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
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

            mantenimientosDataTable.Rows.Add(1, new DateTime(2025, 4, 4), new DateTime(2025, 5, 5), "Empresa Ficticia 1", DBNull.Value, DBNull.Value, 1, "Equipo 1", "Preventivo", "Desc 1");
            mantenimientosDataTable.Rows.Add(2, new DateTime(2026, 1, 1), new DateTime(2026, 2, 2), "Empresa Ficticia 1", DBNull.Value, DBNull.Value, 2, "Equipo 2", "Correctivo", "Desc 2");
            mantenimientosDataTable.Rows.Add(7, new DateTime(2027, 1, 1), new DateTime(2027, 5, 5), "Empresa Ficticia 1", 100.0, "asdadasd", 3, "Equipo 3", "Preventivo", "Desc 3");

            _mantenimientoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(mantenimientosDataTable));

            var resultado = _mantenimientoService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(3));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[1].Id, Is.EqualTo(2));
            Assert.That(resultado.Value[2].Id, Is.EqualTo(7));
            Assert.That(resultado.Value[2].Descripcion, Is.EqualTo("asdadasd"));
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarMantenimientoComando comando = new EliminarMantenimientoComando(7);
            _mantenimientoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _mantenimientoRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarMantenimientoComando>())).Returns(Result<MantenimientoDto>.Success(new MantenimientoDto { Id = 7 }));

            var resultado = _mantenimientoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _mantenimientoRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarMantenimientoComando comando = new EliminarMantenimientoComando(0);

            var resultado = _mantenimientoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
