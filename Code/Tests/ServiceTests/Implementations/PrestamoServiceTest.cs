using Moq;
using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.IO;

namespace IMT_Reservas.Tests.ServiceTests
{
    [TestFixture]
    public class PrestamoServiceTest : IPrestamoServiceTest
    {
        private Mock<IPrestamoRepository> _prestamoRepositoryMock;
        private Mock<MongoDbContexto> _mongoDbContextMock;
        private Mock<IGridFSBucket> _gridFsMock;
        private PrestamoService          _prestamoService;

        [SetUp]
        public void Setup()
        {
            _prestamoRepositoryMock = new Mock<IPrestamoRepository>();
            _mongoDbContextMock = new Mock<MongoDbContexto>();
            _gridFsMock = new Mock<IGridFSBucket>();
            _mongoDbContextMock.Setup(c => c.GestionArchivos).Returns(_gridFsMock.Object);
            _prestamoService = new PrestamoService(_prestamoRepositoryMock.Object, _mongoDbContextMock.Object, _gridFsMock.Object);
        }

        [Test]
        public void CrearPrestamo_ComandoValido_LlamaRepositorioCrear()
        {
            var mockFile = new Mock<IFormFile>();
            var fileContent = "Hello World from a Fake File";
            var fileName = "test.html";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(fileContent);
            writer.Flush();
            ms.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", mockFile.Object);
            var prestamoId = 123;
            var fileId = ObjectId.GenerateNewId();
            var contratoId = ObjectId.GenerateNewId().ToString();

            _prestamoRepositoryMock.Setup(r => r.Crear(comando)).Returns(prestamoId);
            _gridFsMock.Setup(fs => fs.UploadFromStreamAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<GridFSUploadOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fileId);
            
            var mockContratosCollection = new Mock<IMongoCollection<Contrato>>();
            mockContratosCollection.Setup(c => c.InsertOneAsync(It.IsAny<Contrato>(), null, default))
                .Callback<Contrato, InsertOneOptions, CancellationToken>((doc, options, token) => doc.Id = contratoId)
                .Returns(Task.CompletedTask);

            _mongoDbContextMock.Setup(m => m.Contratos).Returns(mockContratosCollection.Object);

            _prestamoService.CrearPrestamo(comando);

            _prestamoRepositoryMock.Verify(r => r.Crear(comando), Times.Once);
            _gridFsMock.Verify(fs => fs.UploadFromStreamAsync(fileName, It.IsAny<Stream>(), null, default), Times.Once);
            mockContratosCollection.Verify(c => c.InsertOneAsync(It.Is<Contrato>(doc => doc.PrestamoId == prestamoId && doc.FileId == fileId.ToString()), null, default), Times.Once);
            _prestamoRepositoryMock.Verify(r => r.ActualizarIdContrato(prestamoId, fileId.ToString()), Times.Once);
        }

        [Test]
        public void CrearPrestamo_ContratoNulo_LanzaArgumentException()
        {
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12890061", null);
            Assert.Throws<ArgumentException>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_CarnetUsuarioVacio_LanzaErrorNombreRequerido()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "", mockFile.Object);
            Assert.Throws<ErrorNombreRequerido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_GrupoEquipoIdNulo_LanzaErrorIdInvalido()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(null, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", mockFile.Object);
            Assert.Throws<ErrorGrupoEquipoIdInvalido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_GrupoEquipoIdVacio_LanzaErrorIdInvalido()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[0], DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "Obs", "12345", mockFile.Object);
            Assert.Throws<ErrorGrupoEquipoIdInvalido>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_FechaDevolucionAnteriorAPrestamo_LanzaArgumentException()
        {
            var mockFile = new Mock<IFormFile>();
            var comando = new CrearPrestamoComando(new int[] { 1 }, DateTime.Now.AddDays(2), DateTime.Now.AddDays(1), "Obs", "12345", mockFile.Object);
            Assert.Throws<ErrorFechaPrestamoYFechaDevolucionInvalidas>(() => _prestamoService.CrearPrestamo(comando));
        }

        [Test]
        public void CrearPrestamo_FechaPrestamoPasada_LanzaArgumentException()
        {
        }

        [Test]
        public void ObtenerTodosPrestamos_CuandoHayDatos_RetornaListaDeDtos()
        {
            DataTable prestamosDataTable = new DataTable();
            prestamosDataTable.Columns.Add("id_prestamo", typeof(int));
            prestamosDataTable.Columns.Add("carnet", typeof(string));
            prestamosDataTable.Columns.Add("nombre", typeof(string));
            prestamosDataTable.Columns.Add("apellido_paterno", typeof(string));
            prestamosDataTable.Columns.Add("telefono", typeof(string));
            prestamosDataTable.Columns.Add("nombre_grupo_equipo", typeof(string));
            prestamosDataTable.Columns.Add("codigo_imt", typeof(string));
            prestamosDataTable.Columns.Add("fecha_solicitud", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_prestamo_esperada", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_prestamo", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_devolucion_esperada", typeof(DateTime));
            prestamosDataTable.Columns.Add("fecha_devolucion", typeof(DateTime));
            prestamosDataTable.Columns.Add("observacion", typeof(string));
            prestamosDataTable.Columns.Add("estado_prestamo", typeof(string));
            prestamosDataTable.Columns.Add("id_contrato", typeof(string));
            
            prestamosDataTable.Rows.Add(5, "12890061", "Juan", "Perez", "777", "Laptop", "L01", new DateTime(2025, 4, 28), new DateTime(2025, 5, 9), new DateTime(2025, 5, 9), new DateTime(2025, 6, 14), DBNull.Value, "Para mi proyecto de grado", "pendiente", "contract1");
            prestamosDataTable.Rows.Add(6, "12890061", "Ana", "Gomez", "888", "Proyector", "P02", new DateTime(2025, 4, 28), new DateTime(2026, 7, 3), new DateTime(2026, 7, 3), new DateTime(2026, 4, 3), DBNull.Value, "Para mi proyecto", "pendiente", "contract2");
            prestamosDataTable.Rows.Add(8, "12890061", "Luis", "Castro", "999", "Tablet", "T03", new DateTime(2025, 4, 29), new DateTime(2029, 5, 4), new DateTime(2029, 5, 4), new DateTime(2029, 7, 6), DBNull.Value, "", "pendiente", "contract3");


            _prestamoRepositoryMock.Setup(r => r.ObtenerTodos()).Returns(prestamosDataTable);

            List<PrestamoDto> resultado = _prestamoService.ObtenerTodosPrestamos();
            Assert.That(resultado, Has.Count.EqualTo(3));
            Assert.That(resultado[0].Id, Is.EqualTo(5));
            Assert.That(resultado[1].Id, Is.EqualTo(6));
            Assert.That(resultado[2].Id, Is.EqualTo(8));
        }

        [Test]
        public void EliminarPrestamo_ComandoValido_LlamaRepositorioEliminar()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(19);
            _prestamoService.EliminarPrestamo(comando);
            _prestamoRepositoryMock.Verify(r => r.Eliminar(comando.Id), Times.Once);
        }

        [Test]
        public void EliminarPrestamo_IdInvalido_LanzaErrorIdInvalido()
        {
            EliminarPrestamoComando comando = new EliminarPrestamoComando(0);
            Assert.Throws<ErrorIdInvalido>(() => _prestamoService.EliminarPrestamo(comando));
        }

        [Test]
        public void AceptarPrestamo_ComandoValido_SubeArchivoYActualizaEstado()
        {
            var mockContratosCollection = new Mock<IMongoCollection<Contrato>>();
            var mockFile = new Mock<IFormFile>();
            var fileContent = "Nuevo contrato";
            var fileName = "contrato_nuevo.html";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(fileContent);
            writer.Flush();
            ms.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var comando = new AceptarPrestamoComando { PrestamoId = 1, Contrato = mockFile.Object };
            var contratoExistente = new Contrato { Id = "abc", PrestamoId = 1, FileId = "oldFileId" };
            _mongoDbContextMock.Setup(m => m.Contratos).Returns(mockContratosCollection.Object);

            var mockCursor = new Mock<IAsyncCursor<Contrato>>();
            mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor.Setup(x => x.Current).Returns(new List<Contrato> { contratoExistente });
            mockContratosCollection.Setup(c => c.FindSync(
                It.IsAny<FilterDefinition<Contrato>>(),
                It.IsAny<FindOptions<Contrato, Contrato>>(),
                It.IsAny<CancellationToken>())).Returns(mockCursor.Object);

            _gridFsMock.Setup(fs => fs.UploadFromStreamAsync(It.IsAny<string>(), It.IsAny<Stream>(), null, default)).ReturnsAsync(ObjectId.GenerateNewId());

            _prestamoService.AceptarPrestamo(comando);

            mockContratosCollection.Verify(c => c.UpdateOne(It.IsAny<FilterDefinition<Contrato>>(), It.IsAny<UpdateDefinition<Contrato>>(), null, default), Times.Once);
            _prestamoRepositoryMock.Verify(r => r.ActualizarEstado(It.Is<ActualizarEstadoPrestamoComando>(cmd => cmd.Id == comando.PrestamoId && cmd.EstadoPrestamo == "activo")), Times.Once);
        }

        [Test]
        public void AceptarPrestamo_ContratoNulo_LanzaArgumentException()
        {
            var comando = new AceptarPrestamoComando { PrestamoId = 1, Contrato = null };

            Assert.Throws<ArgumentException>(() => _prestamoService.AceptarPrestamo(comando));
        }
    }
}
