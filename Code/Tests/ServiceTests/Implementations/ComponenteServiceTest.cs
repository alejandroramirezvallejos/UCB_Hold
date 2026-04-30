using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class ComponenteServiceTest
    {
        private Mock<IComponenteRepository> _componenteRepositoryMock;
        private ComponenteService _componenteService;

        [SetUp]
        public void Setup()
        {
            _componenteRepositoryMock = new Mock<IComponenteRepository>();
            _componenteService = new ComponenteService(_componenteRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 12345, "CPU de 8 núcleos", 350.00, null);
            _componenteRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearComponenteComando>())).Returns(Result<ComponenteDto>.Created(new ComponenteDto { Id = 1, Nombre = "CPU i7" }));

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _componenteRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearComponenteComando comando = new CrearComponenteComando("", "i7-9700K", "Procesador", 12345, "desc", 350.0, null);

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_NombreExcedeLimite_RetornaInvalid()
        {
            CrearComponenteComando comando = new CrearComponenteComando(new string('a', 256), "i7-9700K", "Procesador", 12345, "desc", 350.0, null);

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_ModeloExcedeLimite_RetornaInvalid()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", new string('a', 256), "Procesador", 12345, "desc", 350.0, null);

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_CodigoImtInvalido_RetornaInvalid()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 0, "desc", 350.0, null);

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Crear_PrecioNegativo_RetornaInvalid()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 12345, "desc", -1, null);

            var resultado = _componenteService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable componentesDataTable = new DataTable();
            componentesDataTable.Columns.AddRange(new[]
            {
                new DataColumn("id_componente"),
                new DataColumn("nombre_componente"),
                new DataColumn("modelo_componente"),
                new DataColumn("tipo_componente"),
                new DataColumn("descripcion_componente"),
                new DataColumn("precio_referencia_componente"),
                new DataColumn("nombre_equipo"),
                new DataColumn("codigo_imt_equipo"),
                new DataColumn("url_data_sheet_equipo")
            });

            componentesDataTable.Rows.Add(1, "CPU i7", "i7-9700K", "Procesador", "CPU de 8 núcleos", 350.00, "PC-GAMER-01", 54321, "http://example.com/datasheet.pdf");
            _componenteRepositoryMock.Setup(repository => repository.ObtenerTodos()).Returns(Result<DataTable>.Success(componentesDataTable));

            var resultado = _componenteService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(1));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("CPU i7"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "CPU i9", "i9-9900K", "Procesador", 12345, "CPU de 8 núcleos y 16 hilos", 500.00, null);
            _componenteRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _componenteRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarComponenteComando>())).Returns(Result<ComponenteDto>.Success(new ComponenteDto { Id = 1 }));

            var resultado = _componenteService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _componenteRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(0, "CPU i9", "i9-9900K", "Procesador", 12345, "desc", 500.0, null);

            var resultado = _componenteService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarComponenteComando comando = new EliminarComponenteComando(1);
            _componenteRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _componenteRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarComponenteComando>())).Returns(Result<ComponenteDto>.Success(new ComponenteDto { Id = 1 }));

            var resultado = _componenteService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _componenteRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarComponenteComando comando = new EliminarComponenteComando(0);

            var resultado = _componenteService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
