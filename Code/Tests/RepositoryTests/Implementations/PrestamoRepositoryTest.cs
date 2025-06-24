using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class PrestamoRepositoryTest : IPrestamoRepositoryTest
    {
        private Mock<IExecuteQuery>  _ejecutarConsultaMock;
        private IPrestamoRepository _prestamoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _prestamoRepositorio  = new PrestamoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            var comando = new CrearPrestamoComando(new int[] { 1 }, System.DateTime.Now.AddDays(1), System.DateTime.Now.AddDays(2), "Obs", "12890061", null);
            var dt = new DataTable();
            dt.Columns.Add("id_prestamo", typeof(int));
            dt.Rows.Add(123);

            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(
                It.Is<string>(s => s.Contains("SELECT id_prestamo")),
                It.IsAny<Dictionary<string, object?>>()))
                .Returns(dt);

            var result = _prestamoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarFuncion(
                It.Is<string>(s => s.Contains("SELECT id_prestamo")),
                It.Is<Dictionary<string, object?>>(d => (string)d["carnetUsuario"] == comando.CarnetUsuario)
            ), Times.Once);

            Assert.That(result, Is.EqualTo(123));
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_prestamos")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _prestamoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 19;
            _prestamoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_prestamo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void ActualizarIdContrato_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int prestamoId = 1;
            string contratoId = "xyz";
            _prestamoRepositorio.ActualizarIdContrato(prestamoId, contratoId);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("UPDATE public.prestamos")),
                It.Is<Dictionary<string, object?>>(d => (int)d["idPrestamo"] == prestamoId && (string)d["idContrato"] == contratoId)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new System.Exception("test exception"));
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new System.Exception("test exception"));

            Assert.Throws<ErrorRepository>(() => _prestamoRepositorio.Crear(new CrearPrestamoComando(new int[] { 1 }, System.DateTime.Now, System.DateTime.Now, "Test", "12890061", null)));
            Assert.Throws<ErrorRepository>(() => _prestamoRepositorio.Eliminar(19));
            Assert.Throws<ErrorRepository>(() => _prestamoRepositorio.ActualizarIdContrato(1, "contract-id"));
        }
    }
}
