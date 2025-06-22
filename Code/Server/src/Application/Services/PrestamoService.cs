using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly MongoDbContexto _mongoDbContext;
    private readonly IGridFSBucket _gridFsBucket;

    public PrestamoService(IPrestamoRepository prestamoRepository, MongoDbContexto mongoDbContext, IGridFSBucket gridFsBucket)
    {
        _prestamoRepository = prestamoRepository;
        _mongoDbContext = mongoDbContext;
        _gridFsBucket = gridFsBucket;
    }
    public void CrearPrestamo(CrearPrestamoComando comando)
    {
        ValidarEntradaCreacion(comando);
        if (comando.Contrato == null) throw new ArgumentException("El contrato no puede ser nulo");
        var prestamoId = _prestamoRepository.Crear(comando);
        var fileName = comando.Contrato.FileName;
        using var stream = comando.Contrato.OpenReadStream();
        var fileId = _gridFsBucket.UploadFromStreamAsync(fileName, stream, null, default).GetAwaiter().GetResult();
        var contrato = new Contrato { PrestamoId = prestamoId, FileId = fileId.ToString() };
        _mongoDbContext.Contratos.InsertOneAsync(contrato, null, default).GetAwaiter().GetResult();
        _prestamoRepository.ActualizarIdContrato(prestamoId, contrato.FileId);
    }
    public void EliminarPrestamo(EliminarPrestamoComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _prestamoRepository.Eliminar(comando.Id);
    }
    public List<PrestamoDto>? ObtenerTodosPrestamos()
    {
        DataTable resultado = _prestamoRepository.ObtenerTodos();
        var lista = new List<PrestamoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }
    public void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando)
    {
        ValidarEntradaActualizacionEstado(comando);
        _prestamoRepository.ActualizarEstado(comando);
    }
    public List<PrestamoDto>? ObtenerPrestamosPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo)
    {
        DataTable resultado = _prestamoRepository.ObtenerPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo);
        var lista = new List<PrestamoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }
    public void AceptarPrestamo(AceptarPrestamoComando comando)
    {
        if (comando.Contrato == null) throw new ArgumentException("El contrato no puede ser nulo");
        var fileName = comando.Contrato.FileName;
        using var stream = comando.Contrato.OpenReadStream();
        var fileId = _gridFsBucket.UploadFromStreamAsync(fileName, stream, null, default).GetAwaiter().GetResult();
        var contrato = new Contrato { PrestamoId = comando.PrestamoId, FileId = fileId.ToString() };
        _mongoDbContext.Contratos.UpdateOne(
            Builders<Contrato>.Filter.Eq(x => x.PrestamoId, comando.PrestamoId),
            Builders<Contrato>.Update.Set(x => x.FileId, contrato.FileId));
        _prestamoRepository.ActualizarEstado(new ActualizarEstadoPrestamoComando(comando.PrestamoId, "activo"));
    }
    private void ValidarEntradaCreacion(CrearPrestamoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ErrorNombreRequerido();
        if (comando.GrupoEquipoId == null || comando.GrupoEquipoId.Length == 0) throw new ErrorGrupoEquipoIdInvalido();
        if (comando.GrupoEquipoId.Any(id => id <= 0)) throw new ErrorGrupoEquipoIdInvalido();
        if (comando.FechaPrestamoEsperada == null) throw new ErrorFechaPrestamoEsperadaRequerida();
        if (comando.FechaDevolucionEsperada == null) throw new ErrorFechaDevolucionEsperadaRequerida();
        if (comando.FechaDevolucionEsperada < comando.FechaPrestamoEsperada) throw new ErrorFechaPrestamoYFechaDevolucionInvalidas();
    }
    private void ValidarEntradaEliminacion(EliminarPrestamoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private void ValidarEntradaActualizacionEstado(ActualizarEstadoPrestamoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (string.IsNullOrWhiteSpace(comando.EstadoPrestamo)) throw new ErrorEstadoPrestamoRequerido();
        if (comando.EstadoPrestamo != "pendiente" && comando.EstadoPrestamo != "rechazado" &&
            comando.EstadoPrestamo != "finalizado" && comando.EstadoPrestamo != "cancelado" &&
            comando.EstadoPrestamo != "aprobado" && comando.EstadoPrestamo != "activo")
            throw new ErrorEstadoPrestamoInvalido();
    }
    private static PrestamoDto MapearFilaADto(DataRow fila) => new PrestamoDto
    {
        Id = Convert.ToInt32(fila["id_prestamo"]),
        CarnetUsuario = fila["carnet"] == DBNull.Value ? null : fila["carnet"].ToString(),
        NombreUsuario = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
        ApellidoPaternoUsuario = fila["apellido_paterno"] == DBNull.Value ? null : fila["apellido_paterno"].ToString(),
        TelefonoUsuario = fila["telefono"] == DBNull.Value ? null : fila["telefono"].ToString(),
        NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
        CodigoImt = fila["codigo_imt"] == DBNull.Value ? null : fila["codigo_imt"].ToString(),
        FechaSolicitud = fila["fecha_solicitud"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_solicitud"]),
        FechaPrestamoEsperada = fila["fecha_prestamo_esperada"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo_esperada"]),
        FechaPrestamo = fila["fecha_prestamo"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo"]),
        FechaDevolucionEsperada = fila["fecha_devolucion_esperada"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion_esperada"]),
        FechaDevolucion = fila["fecha_devolucion"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion"]),
        Observacion = fila["observacion"] == DBNull.Value ? null : fila["observacion"].ToString(),
        EstadoPrestamo = fila["estado_prestamo"] == DBNull.Value ? null : fila["estado_prestamo"].ToString(),
    };
}