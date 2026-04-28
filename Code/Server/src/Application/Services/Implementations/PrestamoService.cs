using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.ResponseDTOs;

public class PrestamoService : BaseServicios,
    ICrearServicioResultado<CrearPrestamoComando, PrestamoConEquiposDto>,
    IEliminarServicio<EliminarPrestamoComando>,
    IObtenerTodosServicio<PrestamoDto>
{
    private readonly PrestamoRepository _prestamoRepository;
    
    public PrestamoService(PrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }
    public virtual PrestamoConEquiposDto Crear(CrearPrestamoComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Verificar que el usuario exista
        if (!_prestamoRepository.ExisteUsuarioActivoPorCarnet(comando.CarnetUsuario!))
            throw new ErrorCarnetUsuarioNoEncontrado();

        // Verificar que todos los grupos existan y buscar equipos disponibles
        var equipoIds = new List<int>();
        var equiposInfo = new List<EquipoAsignadoDto>();

        foreach (var grupoId in comando.GrupoEquipoId!)
        {
            if (!_prestamoRepository.ExisteGrupoEquipoActivoPorId(grupoId))
                throw new ErrorRegistroNoEncontrado();

            var idEquipo = _prestamoRepository.ObtenerEquipoDisponiblePorGrupo(
                grupoId, comando.FechaPrestamoEsperada!.Value, comando.FechaDevolucionEsperada!.Value);
            if (idEquipo == null)
                throw new ErrorNoEquiposDisponibles();

            equipoIds.Add(idEquipo.Value);

            // Obtener info del equipo para la respuesta
            var equipoDt = _prestamoRepository.ObtenerEquipoPorId(idEquipo.Value);
            if (equipoDt.Rows.Count > 0)
            {
                var fila = equipoDt.Rows[0];
                equiposInfo.Add(new EquipoAsignadoDto
                {
                    IdEquipo = Convert.ToInt32(fila["id_equipo"]),
                    CodigoImt = fila["codigo_imt"] == DBNull.Value ? null : fila["codigo_imt"].ToString(),
                    CodigoSerial = fila["codigo_serial"] == DBNull.Value ? null : fila["codigo_serial"].ToString(),
                    Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
                    Modelo = fila["modelo"] == DBNull.Value ? null : fila["modelo"].ToString(),
                    Marca = fila["marca"] == DBNull.Value ? null : fila["marca"].ToString(),
                    IdGrupoEquipo = Convert.ToInt32(fila["id_grupo_equipo"])
                });
            }
        }

        // Crear el préstamo
        var idPrestamo = _prestamoRepository.CrearPrestamo(comando);

        // Crear detalles
        foreach (var idEquipo in equipoIds)
        {
            _prestamoRepository.CrearDetallePrestamo(idPrestamo, idEquipo);
        }

        // Guardar contrato si existe
        if (comando.Contrato != null)
        {
            _prestamoRepository.GuardarContrato(idPrestamo, comando.Contrato);
        }

        return new PrestamoConEquiposDto
        {
            IdPrestamo = idPrestamo,
            EquiposAsignados = equiposInfo
        };
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
            if (cmd.Contrato == null) throw new ErrorContratoNoNulo();
        }
    }

    public virtual void Eliminar(EliminarPrestamoComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el préstamo exista y esté activo
        if (!_prestamoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _prestamoRepository.Eliminar(comando);
    }
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando);
        if (comando is EliminarPrestamoComando cmd)
        {
            if (cmd.Id <= 0) throw new ErrorIdInvalido("préstamo");
        }
    }

    public virtual List<PrestamoDto>? ObtenerTodos()
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
        ValidarEntradaActualizacionEstado(comando);

        // Verificar que el préstamo exista
        if (!_prestamoRepository.ExisteActivoPorId(comando.Id!.Value))
            throw new ErrorRegistroNoEncontrado();

        _prestamoRepository.ActualizarEstado(comando);
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