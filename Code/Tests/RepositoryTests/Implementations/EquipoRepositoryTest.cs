using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class EquipoRepositoryTest 
    {
        private Mock<IExecuteQuery> _ejecutarConsultaMock;
        private IEquipoRepository  _equipoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _equipoRepositorio    = new EquipoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int idGrupoEquipo = 1;
            int codigoImt = 1001;
            int? idGavetero = 1;
            CrearEquipoComando comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
            _equipoRepositorio.Crear(idGrupoEquipo, codigoImt, idGavetero, comando);

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

            var resultado = _equipoRepositorio.ObtenerTodos();
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.AreSame(tablaEsperada, resultado.Value);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _equipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 5;
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _equipoRepositorio.Eliminar(new EliminarEquipoComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>() ))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _equipoRepositorio.Crear(1, 1001, 1, new CrearEquipoComando("a", "b", "c", "d", "e", "f", "g", "h", 1, 1, "i")));
            Assert.Throws<Exception>(() => _equipoRepositorio.Actualizar(new ActualizarEquipoComando(1, "a", "b", "c", "d", "e", "f", "g", "h", 1, 1, "i", "j")));
            Assert.Throws<Exception>(() => _equipoRepositorio.Eliminar(new EliminarEquipoComando(5)));
        }
    }
}







