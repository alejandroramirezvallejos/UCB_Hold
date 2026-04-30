using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MuebleServiceTest
    {
        private Mock<IMuebleRepository> _muebleRepositoryMock;
        private MuebleService _muebleService;

        [SetUp]
        public void Setup()
        {
            _muebleRepositoryMock = new Mock<IMuebleRepository>();
            _muebleService = new MuebleService(_muebleRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Armario Metálico", "Almacenamiento", 250.00, "Depósito 2", 180.0, 90.0, 45.0);
            _muebleRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearMuebleComando>())).Returns(Result<MuebleDto>.Created(new MuebleDto { Id = 1, Nombre = "Armario Metálico" }));

            var resultado = _muebleService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _muebleRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearMuebleComando comando = new CrearMuebleComando("", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);

            var resultado = _muebleService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_CostoNegativo_RetornaInvalid()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", -10.0, "Sala 1", 120.0, 60.0, 75.0);

            var resultado = _muebleService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable mueblesDataTable = new DataTable();
            mueblesDataTable.Columns.Add("id_mueble", typeof(int));
            mueblesDataTable.Columns.Add("nombre_mueble", typeof(string));
            mueblesDataTable.Columns.Add("tipo_mueble", typeof(string));
            mueblesDataTable.Columns.Add("ubicacion_mueble", typeof(string));
            mueblesDataTable.Columns.Add("numero_gaveteros_mueble", typeof(int));
            mueblesDataTable.Columns.Add("longitud_mueble", typeof(double));
            mueblesDataTable.Columns.Add("profundidad_mueble", typeof(double));
            mueblesDataTable.Columns.Add("altura_mueble", typeof(double));
            mueblesDataTable.Columns.Add("costo_mueble", typeof(double));

            mueblesDataTable.Rows.Add(4, "ferprueba", "prueba", "x", 0, 1.4, 4.1, 0.5, 100);
            mueblesDataTable.Rows.Add(3, "FERRR", "nd", null, 1, null, null, null, null);

            _muebleRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(mueblesDataTable));

            var resultado = _muebleService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(2));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(4));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("ferprueba"));
            Assert.That(resultado.Value[1].Id, Is.EqualTo(3));
            Assert.That(resultado.Value[1].Nombre, Is.EqualTo("FERRR"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(4, "ferprueba-actualizado", "prueba-v2", 120.00, "y", 1.5, 4.2, 0.6);
            _muebleRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _muebleRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarMuebleComando>())).Returns(Result<MuebleDto>.Success(new MuebleDto { Id = 4 }));

            var resultado = _muebleService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _muebleRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(0, "ferprueba-actualizado", "prueba-v2", 120.00, "y", 1.5, 4.2, 0.6);

            var resultado = _muebleService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarMuebleComando comando = new EliminarMuebleComando(3);
            _muebleRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _muebleRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarMuebleComando>())).Returns(Result<MuebleDto>.Success(new MuebleDto { Id = 3 }));

            var resultado = _muebleService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _muebleRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarMuebleComando comando = new EliminarMuebleComando(0);

            var resultado = _muebleService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
