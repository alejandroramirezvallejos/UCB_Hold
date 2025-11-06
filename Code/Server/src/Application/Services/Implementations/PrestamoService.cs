using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.ResponseDTOs;

public class PrestamoService : BaseServicios, IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    
    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }
    public virtual PrestamoConEquiposDto CrearPrestamo(CrearPrestamoComando comando)
    {
        ValidarEntradaCreacion(comando);
        try
        {
            return _prestamoRepository.Crear(comando);
        }
        catch (ErrorCarnetRequerido) { throw; }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorGrupoEquipoIdInvalido) { throw; }
        catch (ErrorFechaPrestamoYFechaDevolucionInvalidas) { throw; }
        catch (ErrorCarnetUsuarioNoEncontrado) { throw; }
        catch (ErrorNoEquiposDisponibles) { throw; }
        catch (Exception ex) { InterpretarErrorCreacion(comando, ex); throw; }
    }
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando);
        if (comando is CrearPrestamoComando cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.CarnetUsuario)) throw new ErrorCarnetRequerido();
            if (cmd.GrupoEquipoId == null || cmd.GrupoEquipoId.Length == 0) throw new ErrorGrupoEquipoIdInvalido();
            if (cmd.GrupoEquipoId.Any(id => id <= 0)) throw new ErrorGrupoEquipoIdInvalido();
            if (cmd.FechaPrestamoEsperada == null) throw new ErrorFechaPrestamoEsperadaRequerida();
            if (cmd.FechaDevolucionEsperada == null) throw new ErrorFechaDevolucionEsperadaRequerida();
            if (cmd.FechaDevolucionEsperada < cmd.FechaPrestamoEsperada) throw new ErrorFechaPrestamoYFechaDevolucionInvalidas();
        }
    }
    protected override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorCreacion(comando, ex);
        var errorMessage = ex.Message?.ToLower() ?? "";
        if (errorMessage.Contains("error al crear préstamo general: conflicto de llave única")) throw new ErrorRegistroYaExiste();
        if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear el préstamo general")) throw new Exception($"Error inesperado al crear el préstamo general: {ex.Message}", ex);
        if (errorMessage.Contains("fallo crítico: no se pudo obtener el id del préstamo general creado")) throw new Exception("Fallo crítico: No se pudo crear el préstamo", ex);
        if (errorMessage.Contains("grupo id") && errorMessage.Contains("no existe o está eliminado")) throw new ErrorRegistroNoEncontrado();
        if (errorMessage.Contains("no se encontró equipo disponible") && errorMessage.Contains("para el grupo id")) throw new ErrorNoEquiposDisponibles();
        if (errorMessage.Contains("conflicto de llave única al crear detalle")) throw new ErrorRegistroYaExiste();
        if (errorMessage.Contains("violación de restricción check al crear detalle")) throw new Exception($"Error de validación al crear detalle del préstamo: {ex.Message}", ex);
        if (errorMessage.Contains("violación de llave foránea al crear detalle")) throw new ErrorReferenciaInvalida("equipo o préstamo");
        if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear detalle")) throw new Exception($"Error inesperado al crear detalle del préstamo: {ex.Message}", ex);
        if (ex is ErrorDataBase errorDb)
        {
            if (errorDb.SqlState == "23503")
            {
                if (errorMessage.Contains("carnet") || errorMessage.Contains("usuarios")) throw new ErrorCarnetUsuarioNoEncontrado();
                throw new ErrorReferenciaInvalida("referencia de base de datos");
            }
            if (errorDb.SqlState == "23505") throw new ErrorRegistroYaExiste();
            throw new Exception($"Error inesperado de base de datos al crear préstamo: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear préstamo: {errorRepo.Message}", errorRepo);
        throw new Exception($"Error inesperado al crear préstamo: {ex.Message}", ex);
    }   
    public virtual void EliminarPrestamo(EliminarPrestamoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _prestamoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
            throw;
        }
    }
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando);
        if (comando is EliminarPrestamoComando cmd)
        {
            if (cmd.Id <= 0) throw new ErrorIdInvalido("préstamo");
        }
    }
    protected override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorEliminacion(comando, ex);
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un préstamo activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el préstamo")) throw new Exception($"Error inesperado al eliminar préstamo: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar préstamo: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar préstamo: {errorRepo.Message}", errorRepo);
    } 
    public virtual List<PrestamoDto>? ObtenerTodosPrestamos()
    {
        try
        {
            DataTable resultado = _prestamoRepository.ObtenerTodos();
            var lista = new List<PrestamoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as PrestamoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando)
    {
        try
        {
            ValidarEntradaActualizacionEstado(comando);
            _prestamoRepository.ActualizarEstado(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorEstadoPrestamoInvalido) { throw; }
        catch (ErrorEstadoPrestamoRequerido) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no existe préstamo")) throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("error al actualizar el estado del préstamo")) throw new Exception($"Error inesperado al actualizar estado del préstamo: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar estado del préstamo: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar estado del préstamo: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaActualizacionEstado(ActualizarEstadoPrestamoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("préstamo");
        if (string.IsNullOrWhiteSpace(comando.EstadoPrestamo)) throw new ErrorEstadoPrestamoRequerido();
        if (comando.EstadoPrestamo != "pendiente" && comando.EstadoPrestamo != "rechazado" &&
            comando.EstadoPrestamo != "finalizado" && comando.EstadoPrestamo != "cancelado" &&
            comando.EstadoPrestamo != "aprobado" && comando.EstadoPrestamo != "activo")
            throw new ErrorEstadoPrestamoInvalido();
    }   
    public virtual List<PrestamoDto>? ObtenerPrestamosPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo)
    {
        try
        {
            DataTable resultado = _prestamoRepository.ObtenerPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo);
            var lista = new List<PrestamoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as PrestamoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new PrestamoDto
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
            Ubicacion_Equipo = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
            Nombre_Gavetero = fila["nombre_gavetero"] == DBNull.Value ? null : fila["nombre_gavetero"].ToString(),
            Nombre_Mueble = fila["nombre_mueble"] == DBNull.Value ? null : fila["nombre_mueble"].ToString(),
            Ubicacion_Mueble = fila["ubicacion_mueble"] == DBNull.Value ? null : fila["ubicacion_mueble"].ToString()
        };
    }
    public List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta)
    {
        return _prestamoRepository.ObtenerContratoPorPrestamo(consulta);
    }
}