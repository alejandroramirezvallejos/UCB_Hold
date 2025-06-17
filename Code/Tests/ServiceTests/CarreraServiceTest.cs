using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class CarreraServiceTest
    {
        private Mock<CarreraRepository> _carreraRepositoryMock;
        private CarreraService          _carreraService;

        [SetUp]
        public void Setup()
        {
            _carreraRepositoryMock = new Mock<CarreraRepository>();
            _carreraService        = new CarreraService(_carreraRepositoryMock.Object);
        }

        [Test]
        public void CrearCarrera_ComandoValido_LlamaRepositorioCrear()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Ingeniería de Sistemas");
            _carreraService.CrearCarrera(comando);
            _carreraRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearCarrera_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearCarreraComando comando = new CrearCarreraComando("");
            Assert.Throws<ErrorNombreRequerido>(() => _carreraService.CrearCarrera(comando));
        }

        [Test]
        public void CrearCarrera_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            CrearCarreraComando comando = new CrearCarreraComando(new string('a', 101));
            Assert.Throws<ErrorLongitudInvalida>(() => _carreraService.CrearCarrera(comando));
        }

        [Test]
        public void ObtenerTodasCarreras_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable carrerasDataTable = new DataTable();
            carrerasDataTable.Columns.Add("id_carrera", typeof(int));
            carrerasDataTable.Columns.Add("nombre_carrera", typeof(string));
            carrerasDataTable.Rows.Add(1, "Ingeniería de Sistemas");

            _carreraRepositoryMock.Setup(r => r.ObtenerTodas()).Returns(carrerasDataTable);

            List<CarreraDto> resultado = _carreraService.ObtenerTodasCarreras();

            Assert.That(resultado, Has.Exactly(1).Items.Matches<CarreraDto>(carrera =>
                carrera.Id == 1 && carrera.Nombre == "Ingeniería de Sistemas"
            ));
        }

        [Test]
        public void ActualizarCarrera_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "Ingeniería Civil");
            _carreraService.ActualizarCarrera(comando);
            _carreraRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void ActualizarCarrera_IdInvalido_LanzaErrorIdInvalido()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(0, "Ingeniería Civil");
            Assert.Throws<ErrorIdInvalido>(() => _carreraService.ActualizarCarrera(comando));
        }

        [Test]
        public void ActualizarCarrera_NombreVacio_LanzaErrorNombreRequerido()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "");
            Assert.Throws<ErrorNombreRequerido>(() => _carreraService.ActualizarCarrera(comando));
        }

        [Test]
        public void ActualizarCarrera_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, new string('a', 101));
            Assert.Throws<ErrorLongitudInvalida>(() => _carreraService.ActualizarCarrera(comando));
        }

        [Test]
        public void EliminarCarrera_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarCarreraComando comando = new EliminarCarreraComando(1);
            _carreraService.EliminarCarrera(comando);
            _carreraRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarCarrera_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarCarreraComando comando = new EliminarCarreraComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _carreraService.EliminarCarrera(comando));
        }
    }
}

