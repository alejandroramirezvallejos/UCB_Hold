using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class AccesorioServiceTest
    {
        private Mock<AccesorioRepository> _accesorioRepositoryMock;
        private AccesorioService          _accesorioService;

        [SetUp]
        public void Setup()
        {
            _accesorioRepositoryMock = new Mock<AccesorioRepository>();
            _accesorioService        = new AccesorioService(_accesorioRepositoryMock.Object);
        }

        [Test]
        public void CrearAccesorio_ComandoValido_LlamaRepositorioCrear()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Mouse Gamer", "G502", "Periférico", 1001, "Mouse ergonómico", 49.99, null);
            _accesorioService.CrearAccesorio(comando);
            _accesorioRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearAccesorio_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("", "G502", "Periférico", 1001, "desc", 50.0, null);
            Assert.Throws<ErrorNombreRequerido>(() => _accesorioService.CrearAccesorio(comando));
        }

        [Test]
        public void CrearAccesorio_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando(new string('a', 101), "G502", "Periférico", 1001, "desc", 50.0, null);
            Assert.Throws<ErrorLongitudInvalida>(() => _accesorioService.CrearAccesorio(comando));
        }

        [Test]
        public void CrearAccesorio_CodigoImtInvalido_LanzaErrorIdInvalido()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Mouse", "G502", "Periférico", 0, "desc", 50.0, null);
            Assert.Throws<ErrorIdInvalido>(() => _accesorioService.CrearAccesorio(comando));
        }

        [Test]
        public void CrearAccesorio_PrecioNegativo_LanzaErrorValorNegativo()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Mouse", "G502", "Periférico", 1001, "desc", -1, null);
            Assert.Throws<ErrorValorNegativo>(() => _accesorioService.CrearAccesorio(comando));
        }

        [Test]
        public void ObtenerTodosAccesorios_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable accesoriosDataTable = new DataTable();
            accesoriosDataTable.Columns.AddRange(new[]
            {
                new DataColumn("id_accesorio"), new DataColumn("nombre_accesorio"),
                new DataColumn("modelo_accesorio"), new DataColumn("tipo_accesorio"),
                new DataColumn("precio_accesorio"), new DataColumn("nombre_equipo_asociado"),
                new DataColumn("codigo_imt_equipo_asociado"), new DataColumn("descripcion_accesorio")
            });

            accesoriosDataTable.Rows.Add(1, "Mouse Gamer", "G502", "Periférico", 49.99, "PC-01", 12345, "Mouse para gaming");
            _accesorioRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(accesoriosDataTable);

            List<AccesorioDto> resultado = _accesorioService.ObtenerTodosAccesorios();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<AccesorioDto>(accesorio =>
                accesorio.Id == 1 && accesorio.Nombre == "Mouse Gamer" && accesorio.Modelo == "G502" &&
                accesorio.Tipo == "Periférico" && accesorio.Precio == 49.99 && accesorio.NombreEquipoAsociado == "PC-01" &&
                accesorio.CodigoImtEquipoAsociado == 12345 && accesorio.Descripcion == "Mouse para gaming"
            ));
        }

        [Test]
        public void ActualizarAccesorio_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(1, "Mouse Inalámbrico", "MX Master 3", "Periférico", 1002, "Mouse avanzado", 99.99, null);
            _accesorioService.ActualizarAccesorio(comando);
            _accesorioRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void ActualizarAccesorio_IdInvalido_LanzaErrorIdInvalido()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(0, "Mouse", "G502", "Periférico", 1001, "desc", 50.0, null);
            Assert.Throws<ErrorIdInvalido>(() => _accesorioService.ActualizarAccesorio(comando));
        }

        [Test]
        public void EliminarAccesorio_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarAccesorioComando comando = new EliminarAccesorioComando(1);
            _accesorioService.EliminarAccesorio(comando);
            _accesorioRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarAccesorio_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarAccesorioComando comando = new EliminarAccesorioComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _accesorioService.EliminarAccesorio(comando));
        }
    }
}
