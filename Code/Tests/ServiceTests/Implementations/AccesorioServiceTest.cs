using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class AccesorioServiceTest : IAccesorioServiceTest
    {
        private Mock<IAccesorioRepository> _accesorioRepositoryMock;
        private AccesorioService          _accesorioService;

        [SetUp]
        public void Setup()
        {
            _accesorioRepositoryMock = new Mock<IAccesorioRepository>();
            _accesorioService        = new AccesorioService(_accesorioRepositoryMock.Object);
        }

        [Test]
        public void CrearAccesorio_ComandoValido_LlamaRepositorioCrear()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("cable usb", "dasd", "Electrónico", 5, "C-123", 15.99, "https://datasheet.example.com/c123.pdf");
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
            CrearAccesorioComando comando = new CrearAccesorioComando(new string('a', 257), "G502", "Periférico", 1001, "desc", 50.0, null);
            Assert.Throws<ErrorLongitudInvalida>(() => _accesorioService.CrearAccesorio(comando));
        }

        [Test]
        public void CrearAccesorio_CodigoImtInvalido_LanzaErrorIdInvalido()
        {
            Assert.Pass("Este test está implementado como CrearAccesorio_CodigoImtInvalido_LanzaErrorCodigoImtInvalido");
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
                new DataColumn("id_accesorio", typeof(int)), 
                new DataColumn("nombre_accesorio", typeof(string)),
                new DataColumn("modelo_accesorio", typeof(string)), 
                new DataColumn("tipo_accesorio", typeof(string)),
                new DataColumn("precio_accesorio", typeof(double)),
                new DataColumn("nombre_equipo_asociado", typeof(string)),
                new DataColumn("codigo_imt_equipo_asociado", typeof(int)),
                new DataColumn("descripcion_accesorio", typeof(string)),
                new DataColumn("url_data_sheet_accesorio", typeof(string))
            });

            accesoriosDataTable.Rows.Add(2, "cable usb", "dasd", "Electrónico", 15.99, "Equipo A", 123, "C-123", "https://datasheet.example.com/c123.pdf");
            accesoriosDataTable.Rows.Add(3, "string", "string", "string", 777, "Equipo B", 456, "string", null);

            _accesorioRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(accesoriosDataTable);

            List<AccesorioDto> resultado = _accesorioService.ObtenerTodosAccesorios();
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Id, Is.EqualTo(2));
            Assert.That(resultado[0].Nombre, Is.EqualTo("cable usb"));
            Assert.That(resultado[1].Id, Is.EqualTo(3));
            Assert.That(resultado[1].Nombre, Is.EqualTo("string"));
        }

        [Test]
        public void ActualizarAccesorio_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(2, "cable usb-c", "dasd-2", "Electrónico", 5, "cable usb actualizado", 19.99, "https://datasheet.example.com/c123-v2.pdf");
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
            EliminarAccesorioComando comando = new EliminarAccesorioComando(3);
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
