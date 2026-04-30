using Moq;
using System.Data;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using IMT_Reservas.Server.Application.ResponseDTOs;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class PrestamoServiceTest
    {
        private Mock<IPrestamoRepository> _prestamoRepositoryMock;
        private PrestamoService _prestamoService;

        [SetUp]
        public void Setup()
        {
            _prestamoRepositoryMock = new Mock<IPrestamoRepository>();
            _prestamoService = new PrestamoService(_prestamoRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            var mockFile = new Mock<IFormFile>();
            var fileContent = "Hello World from a Fake File";
            var fileName = "test.html";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(fileContent);
            writer.Flush();
            ms.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            var equipoAsignado = new EquipoAsignadoDto { IdEquipo = 1, CodigoImt = "L01" };
            var prestamoDto = new PrestamoConEquiposDto { IdPrestamo = 123, EquiposAsignados = new List<EquipoAsignadoDto> { equipoAsignado } };

            _prestamoRepositoryMock.Setup(r => r.ExisteUsuarioActivoPorCarnet(It.IsAny<string>())).Returns(true);
            _prestamoRepositoryMock.Setup(r => r.ExisteGrupoEquipoActivoPorId(It.IsAny<int>())).Returns(true);
            _prestamoRepositoryMock.Setup(r => r.ObtenerEquipoDisponiblePorGrupo(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(1);
            _prestamoRepositoryMock.Setup(r => r.ObtenerEquipoPorId(It.IsAny<int>())).Returns(CreateEquipoDataTable());
            _prestamoRepositoryMock.Setup(r => r.CrearPrestamo(It.IsAny<CrearPrestamoComando>())).Returns(123);
            _prestamoRepositoryMock.Setup(r => r.CrearDetallePrestamo(It.IsAny<int>(), It.IsAny<int>()));
            _prestamoRepositoryMock.Setup(r => r.GuardarContrato(It.IsAny<int>(), It.IsAny<IFormFile>()));

            var resultado = _prestamoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _prestamoRepositoryMock.Verify(r => r.CrearPrestamo(comando), Times.Once);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable prestamosDataTable = new DataTable();
            prestamosDataTable.Columns.Add("id_prestamo", typeof(int));
            prestamosDataTable.Columns.Add("carnet", typeof(string));
            prestamosDataTable.Columns.Add("nombre", typeof(string));
            prestamosDataTable.Columns.Add("apellido_paterno", typeof(string));
            prestamosDataTable.Columns.Add("telefono", typeof(string));
            prestamosDataTable.Columns.Add("nombre_grupo_equipo", typeof(string));
            prestamosDataTable.Columns.Add("codigo_imt", typeof(string));
            prestamosDataTable.Columns.Add("fecha_solicitud", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_prestamo_esperada", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_prestamo", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_devolucion_esperada", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_devolucion", typeof(DateTime));
            prestamosDataTable.Columns.Add("observacion", typeof(string));
            prestamosDataTable.Columns.Add("estado_prestamo", typeof(string));
            prestamosDataTable.Columns.Add("ubicacion_equipo", typeof(string));
            prestamosDataTable.Columns.Add("nombre_gavetero", typeof(string));
            prestamosDataTable.Columns.Add("nombre_mueble", typeof(string));
            prestamosDataTable.Columns.Add("ubicacion_mueble", typeof(string));

            prestamosDataTable.Rows.Add(5, "12890061", "Juan", "Perez", "777", "Laptop", "L01", new DateTime(2025, 4, 28), new DateTime(2025, 5, 9), new DateTime(2025, 5, 9), new DateTime(2025, 6, 14), DBNull.Value, "Para mi proyecto de grado", "pendiente", "Lab 1", "G1", "M1", "Pasillo");
            prestamosDataTable.Rows.Add(6, "12890061", "Ana", "Gomez", "888", "Proyector", "P02", new DateTime(2025, 4, 28), new DateTime(2026, 7, 3), new DateTime(2026, 7, 3), new DateTime(2026, 4, 3), DBNull.Value, "Para mi proyecto", "pendiente", "Lab 2", "G2", "M2", "Pasillo");
            prestamosDataTable.Rows.Add(8, "12890061", "Luis", "Castro", "999", "Tablet", "T03", new DateTime(2025, 4, 29), new DateTime(2029, 5, 4), new DateTime(2029, 5, 4), new DateTime(2029, 7, 6), DBNull.Value, "", "pendiente", "Lab 3", "G3", "M3", "Pasillo");

            _prestamoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(prestamosDataTable));

            var resultado = _prestamoService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(3));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(5));
            Assert.That(resultado.Value[1].Id, Is.EqualTo(6));
            Assert.That(resultado.Value[2].Id, Is.EqualTo(8));
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(19);
            _prestamoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _prestamoRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarPrestamoComando>())).Returns(Result<PrestamoDto>.Success(new PrestamoDto { Id = 19 }));

            var resultado = _prestamoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _prestamoRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(0);

            var resultado = _prestamoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        private DataTable CreateEquipoDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id_equipo", typeof(int));
            dt.Columns.Add("codigo_imt", typeof(string));
            dt.Columns.Add("codigo_serial", typeof(string));
            dt.Columns.Add("nombre", typeof(string));
            dt.Columns.Add("modelo", typeof(string));
            dt.Columns.Add("marca", typeof(string));
            dt.Columns.Add("id_grupo_equipo", typeof(int));
            dt.Rows.Add(1, "L01", "ABC123", "Laptop", "Dell XPS", "Dell", 1);
            return dt;
        }
    }
}
