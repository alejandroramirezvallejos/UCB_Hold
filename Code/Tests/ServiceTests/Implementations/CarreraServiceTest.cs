using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class CarreraServiceTest 
    {
        private Mock<ICarreraRepository> _carreraRepositoryMock;
        private CarreraService          _carreraService;

        [SetUp]
        public void Setup()
        {
            _carreraRepositoryMock = new Mock<ICarreraRepository>();
            _carreraService        = new CarreraService(_carreraRepositoryMock.Object);
            _carreraRepositoryMock.Setup(r => r.ExisteActivaPorId(It.IsAny<int>())).Returns(true);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearCarreraComando comando = new CrearCarreraComando("Psicopedagogía");
            _carreraRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearCarreraComando>())).Returns(Result<CarreraDto>.Created(new CarreraDto { Id = 1, Nombre = "Psicopedagogía" }));

            var resultado = _carreraService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _carreraRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearCarreraComando comando = new CrearCarreraComando("");

            var resultado = _carreraService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_NombreExcedeLimite_RetornaInvalid()
        {
            CrearCarreraComando comando = new CrearCarreraComando(new string('a', 257));

            var resultado = _carreraService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable carrerasDataTable = new DataTable();
            carrerasDataTable.Columns.Add("id_carrera", typeof(int));
            carrerasDataTable.Columns.Add("nombre_carrera", typeof(string));
            carrerasDataTable.Columns.Add("eliminado", typeof(bool));
            carrerasDataTable.Rows.Add(1, "Mecatronica", false);
            carrerasDataTable.Rows.Add(2, "Software", false);
            carrerasDataTable.Rows.Add(3, "Inteligencia Artificial", false);

            _carreraRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(carrerasDataTable));

            var resultado = _carreraService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(3));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("Mecatronica"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(5, "Ingeniería Civil");
            _carreraRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarCarreraComando>())).Returns(Result<CarreraDto>.Success(new CarreraDto { Id = 5, Nombre = "Ingeniería Civil" }));

            var resultado = _carreraService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _carreraRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(0, "Ingeniería Civil");

            var resultado = _carreraService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Actualizar_NombreVacio_RetornaInvalid()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, "");

            var resultado = _carreraService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Actualizar_NombreExcedeLimite_RetornaInvalid()
        {
            ActualizarCarreraComando comando = new ActualizarCarreraComando(1, new string('a', 256));

            var resultado = _carreraService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarCarreraComando comando = new EliminarCarreraComando(25);
            _carreraRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarCarreraComando>())).Returns(Result<CarreraDto>.Success(new CarreraDto { Id = 25 }));

            var resultado = _carreraService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _carreraRepositoryMock.Verify(r => r.Eliminar(new EliminarCarreraComando(comando.Id)), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarCarreraComando comando = new EliminarCarreraComando(0);

            var resultado = _carreraService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}


