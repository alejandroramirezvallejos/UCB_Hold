using Moq;
using Shared.Common;
using System.Data;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EmpresaMantenimientoServiceTest : IEmpresaMantenimientoServiceTest
    {
        private Mock<EmpresaMantenimientoRepository> _empresaMantenimientoRepositoryMock;
        private EmpresaMantenimientoService          _empresaMantenimientoService;

        [SetUp]
        public void Setup()
        {
            _empresaMantenimientoRepositoryMock = new Mock<EmpresaMantenimientoRepository>();
            _empresaMantenimientoService        = new EmpresaMantenimientoService(_empresaMantenimientoRepositoryMock.Object);
        }

        [Test]
        public void CrearEmpresaMantenimiento_ComandoValido_LlamaRepositorioCrear()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Tech Services", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");
            _empresaMantenimientoService.CrearEmpresaMantenimiento(comando);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void CrearEmpresaMantenimiento_NombreVacio_LanzaErrorNombreRequerido()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");
            Assert.Throws<ErrorNombreRequerido>(() => _empresaMantenimientoService.CrearEmpresaMantenimiento(comando));
        }

        [Test]
        public void ObtenerTodasEmpresasMantenimiento_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable empresasDataTable = new DataTable();
            empresasDataTable.Columns.Add("id_empresa_mantenimiento", typeof(int));
            empresasDataTable.Columns.Add("nombre_empresa", typeof(string));
            empresasDataTable.Columns.Add("nombre_responsable_empresa", typeof(string));
            empresasDataTable.Columns.Add("apellido_responsable_empresa", typeof(string));
            empresasDataTable.Columns.Add("telefono_empresa", typeof(string));
            empresasDataTable.Columns.Add("direccion_empresa", typeof(string));
            empresasDataTable.Columns.Add("nit_empresa", typeof(string));
            empresasDataTable.Rows.Add(1, "Tech Services", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");

            _empresaMantenimientoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(empresasDataTable);

            List<EmpresaMantenimientoDto> resultado = _empresaMantenimientoService.ObtenerTodasEmpresasMantenimiento();
            Assert.That(resultado, Has.Exactly(1).Items.Matches<EmpresaMantenimientoDto>(e => e.Id == 1 && e.NombreEmpresa == "Tech Services"));
        }

        [Test]
        public void ActualizarEmpresaMantenimiento_ComandoValido_LlamaRepositorioActualizar()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "Tech Solutions", "Ana", "Gomez", "87654321", "Av. Siempreviva 742", "8765432-1");
            _empresaMantenimientoService.ActualizarEmpresaMantenimiento(comando);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void EliminarEmpresaMantenimiento_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarEmpresaMantenimientoComando comando = new EliminarEmpresaMantenimientoComando(1);
            _empresaMantenimientoService.EliminarEmpresaMantenimiento(comando);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }
    }
}
