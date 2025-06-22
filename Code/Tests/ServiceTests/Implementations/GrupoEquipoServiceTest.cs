using Moq;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class GrupoEquipoServiceTest : IGrupoEquipoServiceTest
    {
        private Mock<IGrupoEquipoRepository> _grupoEquipoRepositoryMock;
        private GrupoEquipoService          _grupoEquipoService;

        [SetUp]
        public void Setup()
        {
            _grupoEquipoRepositoryMock = new Mock<IGrupoEquipoRepository>();
            _grupoEquipoService        = new GrupoEquipoService(_grupoEquipoRepositoryMock.Object);
        }

        [Test]
        public void CrearGrupoEquipo_ComandoValido_LlamaRepositorioCrear()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Proyectores", "Epson-100", "Epson", "Proyector de alta definición", "Proyectores", null, "http://example.com/img.png");
            _grupoEquipoService.CrearGrupoEquipo(comando);
            _grupoEquipoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearGrupoEquipo_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("", "Epson-100", "Epson", "desc", "cat", null, "img");
            Assert.Throws<ErrorNombreRequerido>(() => _grupoEquipoService.CrearGrupoEquipo(comando));
        }

        [Test]
        public void ObtenerTodosGruposEquipos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable gruposDataTable = new DataTable();
            gruposDataTable.Columns.Add("id_grupo_equipo", typeof(int));
            gruposDataTable.Columns.Add("nombre_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("modelo_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("marca_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("descripcion_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("nombre_categoria", typeof(string));
            gruposDataTable.Columns.Add("url_data_sheet_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("url_imagen_grupo_equipo", typeof(string));
            gruposDataTable.Columns.Add("cantidad_grupo_equipo", typeof(int));
            gruposDataTable.Rows.Add(1, "Proyectores", "Epson-100", "Epson", "desc", "cat", null, "img", 10);

            _grupoEquipoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(gruposDataTable);

            List<GrupoEquipoDto> resultado = _grupoEquipoService.ObtenerTodosGruposEquipos();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<GrupoEquipoDto>(g => g.Id == 1 && g.Nombre == "Proyectores"));
        }

        [Test]
        public void ActualizarGrupoEquipo_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(1, "Proyectores HD", "Epson-200", "Epson", "desc act", "cat", null, "img_act");
            _grupoEquipoService.ActualizarGrupoEquipo(comando);
            _grupoEquipoRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarGrupoEquipo_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarGrupoEquipoComando comando = new EliminarGrupoEquipoComando(1);
            _grupoEquipoService.EliminarGrupoEquipo(comando);
            _grupoEquipoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
