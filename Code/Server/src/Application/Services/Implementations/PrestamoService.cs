using System.Data;
using Ardalis.Result;
using IMT_Reservas.Server.Application.ResponseDTOs;

public class PrestamoService : BaseServicios, IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public virtual Result<PrestamoConEquiposDto> Crear(CrearPrestamoComando comando)
    {
        if (!_prestamoRepository.ExisteUsuarioActivoPorCarnet(comando.CarnetUsuario!))
            return Result<PrestamoConEquiposDto>.NotFound("El usuario no fue encontrado");

        var equipoIds = new List<int>();
        var equiposInfo = new List<EquipoAsignadoDto>();

        foreach (var grupoId in comando.GrupoEquipoId!)
        {
            if (!_prestamoRepository.ExisteGrupoEquipoActivoPorId(grupoId))
                return Result<PrestamoConEquiposDto>.NotFound("El grupo de equipo no fue encontrado");

            var idEquipo = _prestamoRepository.ObtenerEquipoDisponiblePorGrupo(
                grupoId, comando.FechaPrestamoEsperada!.Value, comando.FechaDevolucionEsperada!.Value);
            if (idEquipo == null)
                return Result<PrestamoConEquiposDto>.NotFound("No hay equipos disponibles");

            equipoIds.Add(idEquipo.Value);

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

        var idPrestamo = _prestamoRepository.CrearPrestamo(comando);

        foreach (var idEquipo in equipoIds)
        {
            _prestamoRepository.CrearDetallePrestamo(idPrestamo, idEquipo);
        }

        if (comando.Contrato != null)
        {
            _prestamoRepository.GuardarContrato(idPrestamo, comando.Contrato);
        }

        return Result<PrestamoConEquiposDto>.Created(new PrestamoConEquiposDto
        {
            IdPrestamo = idPrestamo,
            EquiposAsignados = equiposInfo
        });
    }

    public virtual Result<List<PrestamoDto>> ObtenerTodos()
    {
        var repoResult = _prestamoRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<PrestamoDto>>.Error("Error al obtener los préstamos");

        var resultado = repoResult.Value;
        var lista = new List<PrestamoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as PrestamoDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<PrestamoDto>>.NotFound("No se encontraron préstamos")
            : Result<List<PrestamoDto>>.Success(lista);
    }

    public virtual Result<PrestamoDto> Eliminar(EliminarPrestamoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<PrestamoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_prestamoRepository.ExisteActivoPorId(comando.Id))
            return Result<PrestamoDto>.NotFound("El préstamo no fue encontrado");

        var result = _prestamoRepository.Eliminar(comando);
        return result;
    }

    public virtual void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ArgumentException("El ID del préstamo es inválido");
        if (string.IsNullOrWhiteSpace(comando.EstadoPrestamo)) throw new ArgumentException("El estado del préstamo es requerido");

        var isValidState = comando.EstadoPrestamo switch
        {
            "pendiente" or "rechazado" or "finalizado" or "cancelado" or "aprobado" or "activo" => true,
            _ => false
        };
        if (!isValidState)
            throw new ArgumentException("Estado de préstamo inválido");

        if (!_prestamoRepository.ExisteActivoPorId(comando.Id!.Value))
            throw new ArgumentException("El préstamo no fue encontrado");

        _prestamoRepository.ActualizarEstado(comando);
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

    public List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta)
    {
        return _prestamoRepository.ObtenerContratoPorPrestamo(consulta);
    }

    private Result<EliminarPrestamoComando> ValidarEntrada(EliminarPrestamoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarPrestamoComando>.Invalid(errors.ToArray())
            : Result<EliminarPrestamoComando>.Success(comando!);
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
}
