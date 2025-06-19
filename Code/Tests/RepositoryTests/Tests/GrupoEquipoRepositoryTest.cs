using Moq;
using System.Data;

namespace IMT_Reservas.Tests.RepositoryTests
{
    [TestFixture]
    public class GrupoEquipoRepositoryTest : IGrupoEquipoRepositoryTest
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
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png");
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
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https://prueba.com/ds-v2.pdf", "img_act");
            _grupoEquipoRepositorio.Actualizar(comando);

            _ejecutarConsultaMock.Verify(e => e.EjecutarSpNR(
                It.Is<string>(s => s.Contains("actualizar_grupo_equipo")),
                It.Is<Dictionary<string, object?>>(d => (int)d["id"] == comando.Id)
            ), Times.Once);
        }

        [Test]
        public void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos()
        {
            int id = 16;
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

            Assert.Throws<ErrorRepository>(() => _grupoEquipoRepositorio.Crear(new CrearGrupoEquipoComando("Estación de Soldadura", "WES51", "Weller", "Estación de soldadura analógica", "Herramientas", "http://example.com/ds.pdf", "http://example.com/img.png")));
            Assert.Throws<ErrorRepository>(() => _grupoEquipoRepositorio.Actualizar(new ActualizarGrupoEquipoComando(5, "prueba actualizada", "prueba v2", "prueba", "desc act", "Herramientas", "https://prueba.com/ds-v2.pdf", "img_act")));
            Assert.Throws<ErrorRepository>(() => _grupoEquipoRepositorio.Eliminar(16));
        }
    }
}
