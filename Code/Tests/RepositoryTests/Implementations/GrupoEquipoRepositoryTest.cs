using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class GrupoEquipoRepositoryTest 
    {
        private Mock<IExecuteQuery>     _ejecutarConsultaMock;
        private IGrupoEquipoRepository _grupoEquipoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock   = new Mock<IExecuteQuery>();
            _grupoEquipoRepositorio = new GrupoEquipoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int idCategoria = 1;
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png");
            _grupoEquipoRepositorio.Crear(idCategoria, comando);

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

            DataTable resultado = _grupoEquipoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorId_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            tablaEsperada.Rows.Add(); 
            int id = 1;
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable? resultado = _grupoEquipoRepositorio.ObtenerPorId(id);
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorNombreYCategoria_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Columns.Add("Id");
            tablaEsperada.Rows.Add(1);
            string nombre = "Proyector";
            string categoria = "Oficina";
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _grupoEquipoRepositorio.ObtenerPorNombreYCategoria(nombre, categoria);
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https://prueba.com/ds-v2.pdf", "img_act");
            
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _grupoEquipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object?>>()), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 16;
            DataTable existsDt = new DataTable();
            existsDt.Columns.Add("exists", typeof(bool));
            existsDt.Rows.Add(true);
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("EXISTS")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(existsDt);

            _grupoEquipoRepositorio.Eliminar(new EliminarGrupoEquipoComando(id));

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

            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Crear(1, new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png")));
            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Actualizar(1, new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https://prueba.com/ds-v2.pdf", "img_act")));
            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Eliminar(new EliminarGrupoEquipoComando(16)));
        }
    }
}







