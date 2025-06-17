using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class GrupoEquipoRepositoryTest
    {
        private Mock<ExecuteQuery>     _ejecutarConsultaMock;
        private IGrupoEquipoRepository _grupoEquipoRepositorio;

        [SetUp]
        public void Setup()
        {
            _ejecutarConsultaMock   = new Mock<ExecuteQuery>();
            _grupoEquipoRepositorio = new GrupoEquipoRepository(_ejecutarConsultaMock.Object);
        }

        [Test]
        public void Crear_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Proyectores", "Epson-100", "Epson", "desc", "cat", null, "img");
            _grupoEquipoRepositorio.Crear(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("insertar_grupo_equipo")),
                It.Is<Dictionary<string, object?>>(d => (string)d["nombre"] == comando.Nombre)
            ), Times.Once);
        }

        [Test]
        public void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_grupos_equipos")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _grupoEquipoRepositorio.ObtenerTodos();
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorId_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            tablaEsperada.Rows.Add(); 
            int id = 1;
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_grupo_equipo_especifico_por_id")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable? resultado = _grupoEquipoRepositorio.ObtenerPorId(id);
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void ObtenerPorNombreYCategoria_LlamaEjecutarFuncion_YRetornaDataTable()
        {
            DataTable tablaEsperada = new DataTable();
            string nombre = "Proyector";
            string categoria = "Oficina";
            _ejecutarConsultaMock.Setup(e => e.EjecutarFuncion(It.Is<string>(s => s.Contains("obtener_grupos_equipos_por_nombre_y_categoria")), It.IsAny<Dictionary<string, object?>>()))
                           .Returns(tablaEsperada);

            DataTable resultado = _grupoEquipoRepositorio.ObtenerPorNombreYCategoria(nombre, categoria);
            Assert.AreSame(tablaEsperada, resultado);
        }

        [Test]
        public void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(1, "Proyectores HD", "Epson-200", "Epson", "desc act", "cat", null, "img_act");
            _grupoEquipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_grupo_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 1;
            _grupoEquipoRepositorio.Eliminar(id);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("eliminar_grupo_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == id)
            ), Times.Once);
        }

        [Test]
        public void Repositorio_CuandoHayExcepcion_LanzaExcepcion()
        {
            _ejecutarConsultaMock.Setup(e => e.EjecutarSpNR(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                           .Throws(new Exception("test exception"));

            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Crear(new CrearGrupoEquipoComando("Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Actualizar(new ActualizarGrupoEquipoComando(1, "Test", null, null, null, null, null, null)));
            Assert.Throws<Exception>(() => _grupoEquipoRepositorio.Eliminar(1));
        }
    }
}

