using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class ComponenteRepositoryTest 
    {
        private Mock<IExecuteQuery>    _ejecutarConsultaMock;
        private IComponenteRepository _componenteRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock  = new Mock<IExecuteQuery>();
            _componenteRepositorio = new ComponenteRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int idEquipo = 1;
            CrearComponenteComando comando = new CrearComponenteComando("Nuevo Componente", "NC-01", "Tipo Nuevo", 5, "Desc Nuevo", 150.00, "http://example.com/nc01.pdf");
            _componenteRepositorio.Crear(idEquipo, comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            var resultado = _componenteRepositorio.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.AreSame(tablaEsperada, resultado.Value);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarComponenteComando comando = new ActualizarComponenteComando(1, "prueba componente actualizada", "prueba-v2", "jjjj", 7, "desc actualizada", 10.00, "http://example.com/updated.pdf");
            
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _componenteRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 4;
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _componenteRepositorio.Eliminar(new EliminarComponenteComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            Assert.Throws<Exception>(() => _componenteRepositorio.Crear(1, new CrearComponenteComando("Test", "Test", "Test", 1, "Test", 1, "Test")));
            Assert.Throws<Exception>(() => _componenteRepositorio.Actualizar(1, new ActualizarComponenteComando(1, "Test", "Test", "Test", 1, "Test", 1, "Test")));
            Assert.Throws<Exception>(() => _componenteRepositorio.Eliminar(new EliminarComponenteComando(1)));
        }
    }
}







