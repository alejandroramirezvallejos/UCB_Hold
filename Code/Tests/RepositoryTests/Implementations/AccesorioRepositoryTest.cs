using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class AccesorioRepositoryTest 
    {
        private Mock<IExecuteQuery>   _ejecutarConsultaMock;
        private IAccesorioRepository _accesorioRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock = new Mock<IExecuteQuery>();
            _accesorioRepositorio = new AccesorioRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearAccesorioComando comando = new CrearAccesorioComando("cable usb", "dasd", "Electrónico", 123, "C-123", 15.99, "https://datasheet.example.com/c123.pdf");
            _accesorioRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_accesorios")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaResult()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_accesorios")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            var resultado = _accesorioRepositorio.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.AreSame(tablaEsperada, resultado.Value);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarAccesorioComando comando = new ActualizarAccesorioComando(2, "cable usb-c", "dasd-2", "Electrónico", 123, "cable usb actualizado", 19.99, "https://datasheet.example.com/c123-v2.pdf");
            _accesorioRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_accesorio")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 3;
            _accesorioRepositorio.Eliminar(new EliminarAccesorioComando(id));

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_accesorio")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }
    }
}

