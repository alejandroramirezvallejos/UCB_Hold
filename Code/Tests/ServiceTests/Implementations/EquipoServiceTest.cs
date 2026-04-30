using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EquipoServiceTest
    {
        private Mock<IEquipoRepository> _equipoRepositoryMock;
        private Mock<IGrupoEquipoRepository> _grupoEquipoRepositoryMock;
        private EquipoService _equipoService;

        [SetUp]
        public void Setup()
        {
            _equipoRepositoryMock = new Mock<IEquipoRepository>();
            _grupoEquipoRepositoryMock = new Mock<IGrupoEquipoRepository>();
            _equipoService = new EquipoService(_equipoRepositoryMock.Object, _grupoEquipoRepositoryMock.Object);
            _equipoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _equipoRepositoryMock.Setup(r => r.ObtenerEquipoIdPorCodigoImt(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.ObtenerGrupoEquipoIdPorEquipoId(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.ObtenerGaveteroIdPorNombre(It.IsAny<string>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.ObtenerCategoriaIdPorGrupoEquipoId(It.IsAny<int>())).Returns(1);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearEquipoComando comando = new CrearEquipoComando("Osciloscopio", "Tektronix", "TBS1052B", "UCB-OSC-01", "Osciloscopio digital de 2 canales", "SN-OSC-54321", "Laboratorio de Electrónica", "Compra", 450.00, 10, "GAV-03");
            _equipoRepositoryMock.Setup(r => r.ObtenerGrupoEquipoIdPorNombreModeloMarca(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.ObtenerGaveteroIdPorNombre(It.IsAny<string>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.ObtenerCategoriaIdPorGrupoEquipoId(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.GenerarCodigoImt(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.Crear(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CrearEquipoComando>())).Returns(Result<EquipoDto>.Success(new EquipoDto()));

            var resultado = _equipoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _equipoRepositoryMock.Verify(r => r.Crear(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>(), comando), Times.Once);
        }

        [Test]
        public void Crear_NombreGrupoVacio_RetornaInvalid()
        {
            CrearEquipoComando comando = new CrearEquipoComando("", "ThinkPad T480", "Lenovo", null, null, null, null, null, null, null, null);

            var resultado = _equipoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable equiposDataTable = new DataTable();
            equiposDataTable.Columns.AddRange(new[]
            {
                new DataColumn("id_equipo", typeof(int)),
                new DataColumn("nombre_grupo_equipo", typeof(string)),
                new DataColumn("modelo_equipo", typeof(string)),
                new DataColumn("marca_equipo", typeof(string)),
                new DataColumn("codigo_imt_equipo", typeof(int)),
                new DataColumn("codigo_ucb_equipo", typeof(string)),
                new DataColumn("descripcion_equipo", typeof(string)),
                new DataColumn("numero_serial_equipo", typeof(string)),
                new DataColumn("ubicacion_equipo", typeof(string)),
                new DataColumn("procedencia_equipo", typeof(string)),
                new DataColumn("tiempo_max_prestamo_equipo", typeof(int)),
                new DataColumn("nombre_gavetero_equipo", typeof(string)),
                new DataColumn("estado_equipo_equipo", typeof(string)),
                new DataColumn("costo_referencia_equipo", typeof(double))
            });

            equiposDataTable.Rows.Add(2, "Impresoras", "LaserJet Pro", "HP", 33, "JJJJJJ", "Impresora", null, "Pared derecha lab", null, null, null, "operativo", null);
            equiposDataTable.Rows.Add(4, "Fuentes", "DC Power Supply", "Rigol", 35, null, "Fuente de alimentación DC", null, "Pared de frente lab", null, null, null, "operativo", null);
            equiposDataTable.Rows.Add(7, "Pruebas", "Tester", "Fluke", 3, null, "Prueba", null, "No existe", null, 1, "GAV-01", "inoperativo", 400);

            _equipoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(equiposDataTable));

            var resultado = _equipoService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(3));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(2));
            Assert.That(resultado.Value[1].Id, Is.EqualTo(4));
            Assert.That(resultado.Value[2].Id, Is.EqualTo(7));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(7, "Prueba Actualizada", null, null, "UCB-PRUEBA-01", "desc act", "SN-PRUEBA-UPD", "Almacén", "Donación", 450.00, 2, "GAV-01", "operativo");
            _equipoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _equipoRepositoryMock.Setup(r => r.ObtenerGrupoEquipoIdPorEquipoId(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.Actualizar(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<ActualizarEquipoComando>())).Returns(Result<EquipoDto>.Success(new EquipoDto()));

            var resultado = _equipoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _equipoRepositoryMock.Verify(r => r.Actualizar(It.IsAny<int?>(), It.IsAny<int?>(), comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarEquipoComando comando = new ActualizarEquipoComando(0, "Prueba", null, null, "code", "desc", "sn", "ubicacion", "procedencia", 100.0, 1, "gav", "operativo");

            var resultado = _equipoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarEquipoComando comando = new EliminarEquipoComando(5);
            _equipoRepositoryMock.Setup(r => r.ExisteActivoPorId(It.IsAny<int>())).Returns(true);
            _equipoRepositoryMock.Setup(r => r.ObtenerGrupoEquipoIdPorEquipoId(It.IsAny<int>())).Returns(1);
            _equipoRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarEquipoComando>())).Returns(Result<EquipoDto>.Success(new EquipoDto()));

            var resultado = _equipoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _equipoRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarEquipoComando comando = new EliminarEquipoComando(0);

            var resultado = _equipoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}

