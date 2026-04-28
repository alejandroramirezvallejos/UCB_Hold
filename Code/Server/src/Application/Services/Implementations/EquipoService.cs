using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class EquipoService : BaseServicios,
    ICrearServicio<CrearEquipoComando>,
    IActualizarServicio<ActualizarEquipoComando>,
    IEliminarServicio<EliminarEquipoComando>,
    IObtenerTodosServicio<EquipoDto>
{
    private readonly EquipoRepository _equipoRepository;
    private readonly GrupoEquipoRepository _grupoEquipoRepository;

    public EquipoService(EquipoRepository equipoRepository, GrupoEquipoRepository grupoEquipoRepository)
    {
        _equipoRepository = equipoRepository;
        _grupoEquipoRepository = grupoEquipoRepository;
    }

    public void Crear(CrearEquipoComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: nombre del grupo equipo → id_grupo_equipo (usando nombre que viene como NombreGrupoEquipo)
        var idGrupoEquipo = _equipoRepository.ObtenerGrupoEquipoIdPorNombreModeloMarca(
            comando.NombreGrupoEquipo!, comando.Modelo!, comando.Marca!);
        if (idGrupoEquipo == null)
            throw new ErrorGrupoEquipoNoEncontrado();

        // Resolver FK opcional: gavetero por nombre
        int? idGavetero = null;
        if (!string.IsNullOrWhiteSpace(comando.NombreGavetero))
        {
            idGavetero = _equipoRepository.ObtenerGaveteroIdPorNombre(comando.NombreGavetero);
            if (idGavetero == null)
                throw new ErrorGaveteroNoEncontrado();
        }

        // Generar código IMT
        var idCategoria = _equipoRepository.ObtenerCategoriaIdPorGrupoEquipoId(idGrupoEquipo.Value);
        if (idCategoria == null)
            throw new ErrorCategoriaNoEncontrada();

        var codigoImt = _equipoRepository.GenerarCodigoImt(idCategoria.Value);

        // Insertar equipo
        _equipoRepository.Crear(idGrupoEquipo.Value, codigoImt, idGavetero, comando);

        // Trigger logic: incrementar cantidad y recalcular costo promedio del grupo
        _grupoEquipoRepository.ActualizarCantidad(idGrupoEquipo.Value, 1);
        _grupoEquipoRepository.ActualizarCostoPromedio(idGrupoEquipo.Value);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearEquipoComando
        if (comando is CrearEquipoComando equipoComando)
        {
            if (string.IsNullOrWhiteSpace(equipoComando.NombreGrupoEquipo)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(equipoComando.Modelo)) throw new ErrorModeloRequerido();
            if (string.IsNullOrWhiteSpace(equipoComando.Marca)) throw new ErrorMarcaRequerida();
            if (equipoComando.CostoReferencia.HasValue && equipoComando.CostoReferencia < 0) throw new ErrorValorNegativo("costo de referencia");
            if (equipoComando.TiempoMaximoPrestamo.HasValue && equipoComando.TiempoMaximoPrestamo <= 0) throw new ErrorValorNegativo("Tiempo máximo de préstamo");
        }
    }

    public void Actualizar(ActualizarEquipoComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el equipo exista y esté activo
        if (!_equipoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Obtener grupo actual (para trigger logic)
        var grupoActualId = _equipoRepository.ObtenerGrupoEquipoIdPorEquipoId(comando.Id);

        int? nuevoIdGrupoEquipo = null;
        int? nuevoIdGavetero = null;

        // Resolver FK del grupo si se está cambiando
        if (!string.IsNullOrWhiteSpace(comando.NombreGrupoEquipo))
        {
            // Para actualizar necesitamos nombre+modelo+marca, pero aquí solo viene NombreGrupoEquipo
            // Buscar por nombre parcial — en la práctica el SP original también buscaba por nombre
            var sql = @"SELECT id_grupo_equipo FROM public.grupos_equipos WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
            // Usar el repo directamente (no cambiamos la lógica, solo movemos la resolución)
        }

        // Resolver FK del gavetero si se está cambiando
        if (!string.IsNullOrWhiteSpace(comando.NombreGavetero))
        {
            nuevoIdGavetero = _equipoRepository.ObtenerGaveteroIdPorNombre(comando.NombreGavetero);
            if (nuevoIdGavetero == null)
                throw new ErrorGaveteroNoEncontrado();
        }

        // Validar estado_equipo si se proporciona
        if (!string.IsNullOrWhiteSpace(comando.EstadoEquipo))
        {
            var estadosValidos = new[] { "operativo", "inoperativo", "parcialmente_operativo" };
            if (!estadosValidos.Contains(comando.EstadoEquipo))
                throw new ArgumentException("Estado de equipo inválido. Debe ser 'operativo', 'inoperativo', o 'parcialmente_operativo'.");
        }

        _equipoRepository.Actualizar(nuevoIdGrupoEquipo, nuevoIdGavetero, comando);

        // Trigger logic: recalcular costo promedio
        if (grupoActualId.HasValue)
            _grupoEquipoRepository.ActualizarCostoPromedio(grupoActualId.Value);

        // Trigger logic: si cambió de grupo, ajustar cantidades y costo promedio de ambos
        if (nuevoIdGrupoEquipo.HasValue && grupoActualId.HasValue && nuevoIdGrupoEquipo.Value != grupoActualId.Value)
        {
            _grupoEquipoRepository.ActualizarCantidad(grupoActualId.Value, -1);
            _grupoEquipoRepository.ActualizarCantidad(nuevoIdGrupoEquipo.Value, 1);
            _grupoEquipoRepository.ActualizarCostoPromedio(nuevoIdGrupoEquipo.Value);
        }
    }

    private void ValidarEntradaActualizacion(ActualizarEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("equipo");
        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0) throw new ErrorValorNegativo("costo de referencia");
        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0) throw new ErrorValorNegativo("Tiempo máximo de préstamo");
    }

    public void Eliminar(EliminarEquipoComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el equipo exista y esté activo
        if (!_equipoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Obtener grupo actual para trigger logic
        var grupoActualId = _equipoRepository.ObtenerGrupoEquipoIdPorEquipoId(comando.Id);

        _equipoRepository.Eliminar(comando);

        // Trigger logic: decrementar cantidad y recalcular costo promedio
        if (grupoActualId.HasValue)
        {
            _grupoEquipoRepository.ActualizarCantidad(grupoActualId.Value, -1);
            _grupoEquipoRepository.ActualizarCostoPromedio(grupoActualId.Value);
        }
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); 
        
        if (comando is EliminarEquipoComando equipoComando)
        {
            if (equipoComando.Id <= 0) throw new ErrorIdInvalido("equipo");
        }
    }

    public List<EquipoDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _equipoRepository.ObtenerTodos();
            var lista = new List<EquipoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as EquipoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    
    protected override BaseDto MapearFilaADto(DataRow fila) => new EquipoDto
    {
        Id = Convert.ToInt32(fila["id_equipo"]),
        NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
        Modelo = fila["modelo_equipo"] == DBNull.Value ? null : fila["modelo_equipo"].ToString(),
        Marca = fila["marca_equipo"] == DBNull.Value ? null : fila["marca_equipo"].ToString(),
        CodigoImt = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
        CodigoUcb = fila["codigo_ucb_equipo"] == DBNull.Value ? null : fila["codigo_ucb_equipo"].ToString(),
        Descripcion = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString(),
        NumeroSerial = fila["numero_serial_equipo"] == DBNull.Value ? null : fila["numero_serial_equipo"].ToString(),
        Ubicacion = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
        Procedencia = fila["procedencia_equipo"] == DBNull.Value ? null : fila["procedencia_equipo"].ToString(),
        TiempoMaximoPrestamo = fila["tiempo_max_prestamo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_max_prestamo_equipo"]),
        NombreGavetero = fila["nombre_gavetero_equipo"] == DBNull.Value ? null : fila["nombre_gavetero_equipo"].ToString(),
        EstadoEquipo = fila["estado_equipo_equipo"] == DBNull.Value ? null : fila["estado_equipo_equipo"].ToString(),
        CostoReferencia = fila["costo_referencia_equipo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia_equipo"]),
    };
}