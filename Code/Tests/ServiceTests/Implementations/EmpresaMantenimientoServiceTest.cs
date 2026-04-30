using Moq;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class EmpresaMantenimientoServiceTest
    {
        private Mock<IEmpresaMantenimientoRepository> _empresaMantenimientoRepositoryMock;
        private EmpresaMantenimientoService _empresaMantenimientoService;

        [SetUp]
        public void Setup()
        {
            _empresaMantenimientoRepositoryMock = new Mock<IEmpresaMantenimientoRepository>();
            _empresaMantenimientoService = new EmpresaMantenimientoService(_empresaMantenimientoRepositoryMock.Object);
        }

        [Test]
        public void Crear_ComandoValido_RetornaSuccess()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("Tech Services", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");
            _empresaMantenimientoRepositoryMock.Setup(r => r.Crear(It.IsAny<CrearEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Created(new EmpresaMantenimientoDto { Id = 1, NombreEmpresa = "Tech Services" }));

            var resultado = _empresaMantenimientoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
        }

        [Test]
        public void Crear_NombreVacio_RetornaInvalid()
        {
            CrearEmpresaMantenimientoComando comando = new CrearEmpresaMantenimientoComando("", "Juan", "Perez", "12345678", "Calle Falsa 123", "1234567-8");

            var resultado = _empresaMantenimientoService.Crear(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void ObtenerTodos_CuandoHayDatos_RetornaListaDeDtos()
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

            _empresaMantenimientoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(Result<DataTable>.Success(empresasDataTable));

            var resultado = _empresaMantenimientoService.ObtenerTodos();

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Value, Has.Count.EqualTo(1));
            Assert.That(resultado.Value[0].Id, Is.EqualTo(1));
            Assert.That(resultado.Value[0].NombreEmpresa, Is.EqualTo("Tech Services"));
        }

        [Test]
        public void Actualizar_ComandoValido_RetornaSuccess()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(1, "Tech Solutions", "Ana", "Gomez", "87654321", "Av. Siempreviva 742", "8765432-1");
            _empresaMantenimientoRepositoryMock.Setup(r => r.ExisteActivaPorId(It.IsAny<int>())).Returns(true);
            _empresaMantenimientoRepositoryMock.Setup(r => r.Actualizar(It.IsAny<ActualizarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Success(new EmpresaMantenimientoDto { Id = 1 }));

            var resultado = _empresaMantenimientoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Actualizar(comando), Times.Once);
        }

        [Test]
        public void Actualizar_IdInvalido_RetornaInvalid()
        {
            ActualizarEmpresaMantenimientoComando comando = new ActualizarEmpresaMantenimientoComando(0, "Tech Solutions", "Ana", "Gomez", "87654321", "Av. Siempreviva 742", "8765432-1");

            var resultado = _empresaMantenimientoService.Actualizar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }

        [Test]
        public void Eliminar_ComandoValido_RetornaSuccess()
        {
            EliminarEmpresaMantenimientoComando comando = new EliminarEmpresaMantenimientoComando(1);
            _empresaMantenimientoRepositoryMock.Setup(r => r.ExisteActivaPorId(It.IsAny<int>())).Returns(true);
            _empresaMantenimientoRepositoryMock.Setup(r => r.Eliminar(It.IsAny<EliminarEmpresaMantenimientoComando>())).Returns(Result<EmpresaMantenimientoDto>.Success(new EmpresaMantenimientoDto { Id = 1 }));

            var resultado = _empresaMantenimientoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.True);
            _empresaMantenimientoRepositoryMock.Verify(r => r.Eliminar(comando), Times.Once);
        }

        [Test]
        public void Eliminar_IdInvalido_RetornaInvalid()
        {
            EliminarEmpresaMantenimientoComando comando = new EliminarEmpresaMantenimientoComando(0);

            var resultado = _empresaMantenimientoService.Eliminar(comando);

            Assert.That(resultado.IsSuccess, Is.False);
        }
    }
}
