using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class PrestamoServiceTest
    {
        private Mock<PrestamoRepository> _prestamoRepositoryMock;
        private PrestamoService          _prestamoService;

        [SetUp]
        public void Setup()
        {
            _prestamoRepositoryMock = new Mock<PrestamoRepository>();
            _prestamoService        = new PrestamoService(_prestamoRepositoryMock.Object);
        }

        [Test]
        public void CrearPrestamo_ComandoValido_LlamaRepositorioCrear()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", null);
            _prestamoService.CrearPrestamo(comando);
            _prestamoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearPrestamo_CarnetUsuarioVacio_LanzaErrorNombreRequerido()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "", null);
            Assert.Throws<ErrorNombreRequerido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_GrupoEquipoIdNulo_LanzaErrorIdInvalido()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(null, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", null);
            Assert.Throws<ErrorIdInvalido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_GrupoEquipoIdVacio_LanzaErrorIdInvalido()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[0], DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", null);
            Assert.Throws<ErrorIdInvalido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_FechaPrestamoPasada_LanzaArgumentException()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(2), "Obs", "12345", null);
            Assert.Throws<ArgumentException>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_FechaDevolucionAnteriorAPrestamo_LanzaArgumentException()
        {
            CrearPrestamoComando comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(2), DateTime.Now.AddDays(1), "Obs", "12345", null);
            Assert.Throws<ArgumentException>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void ObtenerTodosPrestamos_CuandoHayDatos_RetornaListaDeDtos()
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
            prestamosDataTable.Rows.Add(1, "12345", "Juan", "Perez", "777", "Laptop", "L01", DateTime.Now, DateTime.Now.AddDays(1), null, DateTime.Now.AddDays(2), null, "obs", "Pendiente");

            _prestamoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(prestamosDataTable);

            List<PrestamoDto> resultado = _prestamoService.ObtenerTodosPrestamos();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<PrestamoDto>(p => p.Id == 1 && p.CarnetUsuario == "12345"));
        }

        [Test]
        public void EliminarPrestamo_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(1);
            _prestamoService.EliminarPrestamo(comando);
            _prestamoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarPrestamo_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _prestamoService.EliminarPrestamo(comando));
        }
    }
}

