using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class ComponenteServiceTest
    {
        private Mock<ComponenteRepository> _componenteRepositoryMock;
        private ComponenteService          _componenteService;

        [SetUp]
        public void Setup()
        {
            _componenteRepositoryMock = new Mock<ComponenteRepository>();
            _componenteService        = new ComponenteService(_componenteRepositoryMock.Object);
        }

        [Test]
        public void CrearComponente_ComandoValido_LlamaRepositorioCrear()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 12345, "CPU de 8 núcleos", 350.00, null);
            _componenteService.CrearComponente(comando);
            _componenteRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearComponente_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearComponenteComando comando = new CrearComponenteComando("", "i7-9700K", "Procesador", 12345, "desc", 350.0, null);
            Assert.Throws<ErrorNombreRequerido>(() => _componenteService.CrearComponente(comando));
        }

        [Test]
        public void CrearComponente_NombreExcedeLimite_LanzaErrorLongitudInvalida()
        {
            CrearComponenteComando comando = new CrearComponenteComando(new string('a', 101), "i7-9700K", "Procesador", 12345, "desc", 350.0, null);
            Assert.Throws<ErrorLongitudInvalida>(() => _componenteService.CrearComponente(comando));
        }

        [Test]
        public void CrearComponente_ModeloExcedeLimite_LanzaErrorLongitudInvalida()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", new string('a', 51), "Procesador", 12345, "desc", 350.0, null);
            Assert.Throws<ErrorLongitudInvalida>(() => _componenteService.CrearComponente(comando));
        }

        [Test]
        public void CrearComponente_CodigoImtInvalido_LanzaErrorIdInvalido()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 0, "desc", 350.0, null);
            Assert.Throws<ErrorIdInvalido>(() => _componenteService.CrearComponente(comando));
        }

        [Test]
        public void CrearComponente_PrecioNegativo_LanzaErrorValorNegativo()
        {
            CrearComponenteComando comando = new CrearComponenteComando("CPU i7", "i7-9700K", "Procesador", 12345, "desc", -1, null);
            Assert.Throws<ErrorValorNegativo>(() => _componenteService.CrearComponente(comando));
        }

        [Test]
        public void ObtenerTodosComponentes_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable componentesDataTable = new DataTable();
            componentesDataTable.Columns.AddRange(new[]
            {
                new DataColumn("id_componente"), new DataColumn("nombre_componente"),
                new DataColumn("modelo_componente"), new DataColumn("tipo_componente"),
                new DataColumn("descripcion_componente"), new DataColumn("precio_referencia_componente"),
                new DataColumn("nombre_equipo"), new DataColumn("codigo_imt_equipo")
            });
            
            componentesDataTable.Rows.Add(1, "CPU i7", "i7-9700K", "Procesador", "CPU de 8 núcleos", 350.00, "PC-GAMER-01", 54321);
            _componenteRepositoryMock.Setup(repository => repository.ObtenerTodos()).Returns(componentesDataTable);

            List<ComponenteDto> resultado = _componenteService.ObtenerTodosComponentes();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<ComponenteDto>(componente =>
                componente.Id == 1 &&
                componente.Nombre == "CPU i7" &&
                componente.Modelo == "i7-9700K" &&
                componente.Tipo == "Procesador" &&
                componente.Descripcion == "CPU de 8 núcleos" &&
                componente.PrecioReferencia == 350.00 &&
                componente.NombreEquipo == "PC-GAMER-01" &&
                componente.CodigoImtEquipo == 54321
            ));
        }

        [Test]
        public void ActualizarComponente_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "CPU i9", "i9-9900K", "Procesador", 12345, "CPU de 8 núcleos y 16 hilos", 500.00, null);
            _componenteService.ActualizarComponente(comando);
            _componenteRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void ActualizarComponente_IdInvalido_LanzaErrorIdInvalido()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(0, "CPU i9", "i9-9900K", "Procesador", 12345, "desc", 500.0, null);
            Assert.Throws<ErrorIdInvalido>(() => _componenteService.ActualizarComponente(comando));
        }

        [Test]
        public void EliminarComponente_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarComponenteComando comando = new EliminarComponenteComando(1);
            _componenteService.EliminarComponente(comando);
            _componenteRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarComponente_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarComponenteComando comando = new EliminarComponenteComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _componenteService.EliminarComponente(comando));
        }
    }
}
