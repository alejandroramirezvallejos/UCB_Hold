using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class GrupoEquipoServiceTest
    {
        private Mock<IGrupoEquipoRepository> _grupoEquipoRepositoryMock;
        private GrupoEquipoService _grupoEquipoService;

        [SetUp]
        public void Setup()
        {
            _grupoEquipoRepositoryMock = new Mock<IGrupoEquipoRepository>();
            _grupoEquipoService = new GrupoEquipoService(_grupoEquipoRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("Proyectores", "Epson-100", "Epson", "Proyector de alta definición", "Proyectores", null, "http://example.com/img.png");
            _grupoEquipoRepositoryMock.Setup(r => r.ObtenerCategoriaIdPorNombre(It.IsAny<string>())).Returns(1);
            _grupoEquipoRepositoryMock.Setup(r => r.ExisteDuplicadoPorNombreModeloMarca(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _grupoEquipoRepositoryMock.Setup(r => r.Crear(It.IsAny<int>(), It.IsAny<CrearGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Created(new GrupoEquipoDto { Id = 1, Nombre = "Proyectores" }));

            var resultado = _grupoEquipoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _grupoEquipoRepositoryMock.Verify(r => r.Crear(It.IsAny<int>(), comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearGrupoEquipoComando comando = new CrearGrupoEquipoComando("", "Epson-100", "Epson", "desc", "cat", null, "img");

            var resultado = _grupoEquipoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
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
            gruposDataTable.Columns.Add("costo_promedio", typeof(decimal));
            gruposDataTable.Rows.Add(1, "Proyectores", "Epson-100", "Epson", "desc", "cat", null, "img", 10, 100.50m);

            _grupoEquipoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(gruposDataTable));

            var resultado = _grupoEquipoService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(1));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[0].Nombre, Is.EqualTo("Proyectores"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(1, "Proyectores HD", "Epson-200", "Epson", "desc act", "cat", null, "img_act");
            _grupoEquipoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _grupoEquipoRepositoryMock.Setup(r => r.Actualizar(It.IsAny<int?>(), It.IsAny<ActualizarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Success(new GrupoEquipoDto { Id = 1 }));

            var resultado = _grupoEquipoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _grupoEquipoRepositoryMock.Verify(r => r.Actualizar(It.IsAny<int?>(), comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarGrupoEquipoComando comando = new ActualizarGrupoEquipoComando(0, "Proyectores HD", "Epson-200", "Epson", "desc act", "cat", null, "img_act");

            var resultado = _grupoEquipoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarGrupoEquipoComando comando = new EliminarGrupoEquipoComando(1);
            _grupoEquipoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _grupoEquipoRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarGrupoEquipoComando>())).Returns(Result<GrupoEquipoDto>.Success(new GrupoEquipoDto { Id = 1 }));

            var resultado = _grupoEquipoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _grupoEquipoRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarGrupoEquipoComando comando = new EliminarGrupoEquipoComando(0);

            var resultado = _grupoEquipoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
