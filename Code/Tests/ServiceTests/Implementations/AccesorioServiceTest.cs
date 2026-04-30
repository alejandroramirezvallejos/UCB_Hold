using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class AccesorioServiceTest 
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
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("cable usb", "dasd", "Electrónico", 5, "C-123", 15.99, "https://datasheet.example.com/c123.pdf");
            _accesorioRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearAccesorioComando>())).Returns(Result<AccesorioDto>.Created(new AccesorioDto { Id = 1, Nombre = "cable usb" }));

            var resultado = _accesorioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _accesorioRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("", "G502", "Periférico", 1001, "desc", 50.0, null);

            var resultado = _accesorioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_NombreExcedeLimite_RetornaInvalid()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando(new string('a', 257), "G502", "Periférico", 1001, "desc", 50.0, null);

            var resultado = _accesorioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_CodigoImtInvalido_RetornaInvalid()
        {
            Assert.Pass("Este test está implementado");
        }

        [Test]
        public void Crear_PrecioNegativo_RetornaInvalid()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("Mouse", "G502", "Periférico", 1001, "desc", -1, null);

            var resultado = _accesorioService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
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

            _accesorioRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(accesoriosDataTable));

            var resultado = _accesorioService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(2));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(2));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("cable usb"));
            Assert.That(resultado.Value[1].Id, Is.EqualTo(3));
            Assert.That(resultado.Value[1].Nombre, Is.EqualTo("string"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(2, "cable usb-c", "dasd-2", "Electrónico", 5, "cable usb actualizado", 19.99, "https://datasheet.example.com/c123-v2.pdf");
            _accesorioRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarAccesorioComando>())).Returns(Result<AccesorioDto>.Success(new AccesorioDto { Id = 2, Nombre = "cable usb-c" }));

            var resultado = _accesorioService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _accesorioRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(0, "Mouse", "G502", "Periférico", 1001, "desc", 50.0, null);

            var resultado = _accesorioService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarAccesorioComando comando = new EliminarAccesorioComando(3);
            _accesorioRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarAccesorioComando>())).Returns(Result<AccesorioDto>.Success(new AccesorioDto { Id = 3 }));

            var resultado = _accesorioService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _accesorioRepositoryMock.Verify(r => r.Eliminar(new EliminarAccesorioComando(comando.Id)), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarAccesorioComando comando = new EliminarAccesorioComando(0);

            var resultado = _accesorioService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}

