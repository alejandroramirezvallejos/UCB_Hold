using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class MuebleServiceTest : IMuebleServiceTest
    {
        private Mock<MuebleRepository> _muebleRepositoryMock;
        private MuebleService          _muebleService;

        [SetUp]
        public void Setup()
        {
            _muebleRepositoryMock = new Mock<MuebleRepository>();
            _muebleService        = new MuebleService(_muebleRepositoryMock.Object);
        }

        [Test]
        public void CrearMueble_ComandoValido_LlamaRepositorioCrear()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Armario Metálico", "Almacenamiento", 250.00, "Depósito 2", 180.0, 90.0, 45.0);
            _muebleService.CrearMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearMueble_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearMuebleComando comando = new CrearMuebleComando("", "Oficina", 150.00, "Sala 1", 120.0, 60.0, 75.0);
            Assert.Throws<ErrorNombreRequerido>(() => _muebleService.CrearMueble(comando));
        }

        [Test]
        public void CrearMueble_CostoNegativo_LanzaErrorValorNegativo()
        {
            CrearMuebleComando comando = new CrearMuebleComando("Escritorio", "Oficina", -10.0, "Sala 1", 120.0, 60.0, 75.0);
            Assert.Throws<ErrorValorNegativo>(() => _muebleService.CrearMueble(comando));
        }

        [Test]
        public void ObtenerTodosMuebles_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable mueblesDataTable = new DataTable();
            mueblesDataTable.Columns.Add("id_mueble", typeof(int));
            mueblesDataTable.Columns.Add("nombre", typeof(string));
            mueblesDataTable.Columns.Add("tipo", typeof(string));
            mueblesDataTable.Columns.Add("ubicacion", typeof(string));
            mueblesDataTable.Columns.Add("numero_gaveteros", typeof(int));
            mueblesDataTable.Columns.Add("estado_eliminado", typeof(bool));
            mueblesDataTable.Columns.Add("longitud", typeof(double));
            mueblesDataTable.Columns.Add("profundidad", typeof(double));
            mueblesDataTable.Columns.Add("altura", typeof(double));
            mueblesDataTable.Columns.Add("costo", typeof(double));

            mueblesDataTable.Rows.Add(4, "ferprueba", "prueba", "x", 0, false, 1.4, 4.1, 0.5, 100);
            mueblesDataTable.Rows.Add(3, "FERRR", "nd", null, 1, false, null, null, null, null);

            _muebleRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(mueblesDataTable);

            List<MuebleDto> resultado = _muebleService.ObtenerTodosMuebles();
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Id, Is.EqualTo(4));
            Assert.That(resultado[0].Nombre, Is.EqualTo("ferprueba"));
            Assert.That(resultado[1].Id, Is.EqualTo(3));
            Assert.That(resultado[1].Nombre, Is.EqualTo("FERRR"));
        }

        [Test]
        public void ActualizarMueble_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarMuebleComando comando = new ActualizarMuebleComando(4, "ferprueba-actualizado", "prueba-v2", 120.00, "y", 1.5, 4.2, 0.6);
            _muebleService.ActualizarMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarMueble_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarMuebleComando comando = new EliminarMuebleComando(3);
            _muebleService.EliminarMueble(comando);
            _muebleRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
