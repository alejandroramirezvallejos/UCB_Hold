using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MantenimientoService : BaseServicios,
    ICrearServicio<CrearMantenimientoComando>,
    IEliminarServicio<EliminarMantenimientoComando>,
    IObtenerTodosServicio<MantenimientoDto>
{
    private readonly MantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(MantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }

    public virtual void Crear(CrearMantenimientoComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: nombre de empresa → id_empresa
        var idEmpresa = _mantenimientoRepository.ObtenerEmpresaIdPorNombre(comando.NombreEmpresaMantenimiento!);
        if (idEmpresa == null)
            throw new ErrorEmpresaMantenimientoNoEncontrada();

        // Validar que todos los códigos IMT correspondan a equipos activos
        var equipoIds = new int[comando.CodigoIMT!.Length];
        for (int i = 0; i < comando.CodigoIMT.Length; i++)
        {
            var idEquipo = _mantenimientoRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT[i]);
            if (idEquipo == null)
                throw new ErrorCodigoImtNoEncontrado();
            equipoIds[i] = idEquipo.Value;
        }

        // Crear el mantenimiento (retorna ID)
        var idMantenimiento = _mantenimientoRepository.CrearMantenimiento(idEmpresa.Value, comando);

        // Crear detalles de mantenimiento
        for (int i = 0; i < equipoIds.Length; i++)
        {
            var tipo = comando.TipoMantenimiento != null && i < comando.TipoMantenimiento.Length
                ? comando.TipoMantenimiento[i] : null;
            var desc = comando.DescripcionEquipo != null && i < comando.DescripcionEquipo.Length
                ? comando.DescripcionEquipo[i] : null;
            _mantenimientoRepository.CrearDetalleMantenimiento(idMantenimiento, equipoIds[i], tipo, desc);
        }
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearMantenimientoComando
        if (comando is CrearMantenimientoComando mantenimientoComando)
        {
            if (mantenimientoComando.FechaMantenimiento == null) throw new ErrorFechaMantenimientoInicioRequerida();
            if (mantenimientoComando.FechaFinalDeMantenimiento == null) throw new ErrorFechaMantenimientoFinalRequerida();
            if (mantenimientoComando.FechaFinalDeMantenimiento < mantenimientoComando.FechaMantenimiento) throw new ErrorFechaInvalida();
            if (string.IsNullOrWhiteSpace(mantenimientoComando.NombreEmpresaMantenimiento)) throw new ErrorNombreRequerido();
            if (mantenimientoComando.CodigoIMT == null || mantenimientoComando.CodigoIMT.Length == 0) throw new ErrorCodigoImtRequerido();
            if (mantenimientoComando.TipoMantenimiento == null || mantenimientoComando.TipoMantenimiento.Length == 0) throw new ErrorTipoMantenimientoRequerido();
            if (mantenimientoComando.CodigoIMT.Length != mantenimientoComando.TipoMantenimiento.Length) throw new ErrorCodigoImtYTiposLongitudDiferente();
            if (mantenimientoComando.CodigoIMT.Any(codigo => codigo <= 0)) throw new ErrorCodigosImtInvalido();
            if (mantenimientoComando.Costo.HasValue && mantenimientoComando.Costo.Value < 0) throw new ErrorValorNegativo("costo");
        }
    }

    public virtual void Eliminar(EliminarMantenimientoComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el mantenimiento exista y esté activo
        if (!_mantenimientoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _mantenimientoRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarMantenimientoComando
        if (comando is EliminarMantenimientoComando mantenimientoComando)
        {
            if (mantenimientoComando.Id <= 0) throw new ErrorIdInvalido("mantenimiento");
        }
    }

    public virtual List<MantenimientoDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _mantenimientoRepository.ObtenerTodos();
            var lista = new List<MantenimientoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows) 
            {
                var dto = MapearFilaADto(fila) as MantenimientoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    
    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new MantenimientoDto
        {
            Id = Convert.ToInt32(fila["id_mantenimiento"]),
            FechaMantenimiento = fila["fecha_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_mantenimiento"])),
            FechaFinalDeMantenimiento = fila["fecha_final_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_final_mantenimiento"])),
            NombreEmpresaMantenimiento = fila["nombre_empresa_mantenimiento"] == DBNull.Value ? null : fila["nombre_empresa_mantenimiento"].ToString(),
            Costo = fila["costo_mantenimiento"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mantenimiento"]),
            Descripcion = fila["descripcion_mantenimiento"] == DBNull.Value ? null : fila["descripcion_mantenimiento"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            TipoMantenimiento = fila["tipo_detalle_mantenimiento"] == DBNull.Value ? null : fila["tipo_detalle_mantenimiento"].ToString(),
            DescripcionEquipo = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString()
        };
    }
}