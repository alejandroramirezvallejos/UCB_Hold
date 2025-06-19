using Moq;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class EquipoRepositoryTest : IEquipoRepositoryTest
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
            CrearEquipoComando comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
            _equipoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.NombreGrupoEquipo)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_equipos")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _equipoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            _equipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 5;
            _equipoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<ErrorRepository>(() => _equipoRepositorio.Crear(new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03")));
            Assert.Throws<ErrorRepository>(() => _equipoRepositorio.Actualizar(new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo")));
            Assert.Throws<ErrorRepository>(() => _equipoRepositorio.Eliminar(5));
        }
    }
}
