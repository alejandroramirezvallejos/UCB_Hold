using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MantenimientoService : IMantenimientoService
{
    private readonly IMantenimientoRepository _mantenimientoRepository;
    public MantenimientoService(IMantenimientoRepository mantenimientoRepository) => _mantenimientoRepository = mantenimientoRepository;

    public void CrearMantenimiento(CrearMantenimientoComando comando)
    {
        ValidarEntradaCreacion(comando);
        _mantenimientoRepository.Crear(comando);
    }

    public void EliminarMantenimiento(EliminarMantenimientoComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _mantenimientoRepository.Eliminar(comando.Id);
    }

    public List<MantenimientoDto>? ObtenerTodosMantenimientos()
    {
        var resultado = _mantenimientoRepository.ObtenerTodos();
        var lista = new List<MantenimientoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    private void ValidarEntradaCreacion(CrearMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.FechaMantenimiento == null) throw new ErrorFechaMantenimientoInicioRequerida();
        if (comando.FechaFinalDeMantenimiento == null) throw new ErrorFechaMantenimientoFinalRequerida();
        if (comando.FechaFinalDeMantenimiento < comando.FechaMantenimiento) throw new ErrorFechaInvalida();
        if (string.IsNullOrWhiteSpace(comando.NombreEmpresaMantenimiento)) throw new ErrorNombreRequerido();
        if (comando.CodigoIMT == null || comando.CodigoIMT.Length == 0) throw new ErrorCodigoImtRequerido();
        if (comando.TipoMantenimiento == null || comando.TipoMantenimiento.Length == 0) throw new ErrorTipoMantenimientoRequerido();
        if (comando.CodigoIMT.Length != comando.TipoMantenimiento.Length) throw new ErrorCodigoImtYTiposLongitudDiferente();
        if (comando.CodigoIMT.Any(codigo => codigo <= 0)) throw new ErrorCodigosImtInvalido();
        if (comando.Costo.HasValue && comando.Costo.Value < 0) throw new ErrorValorNegativo("costo");
    }
    private void ValidarEntradaEliminacion(EliminarMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static MantenimientoDto MapearFilaADto(DataRow fila) => new MantenimientoDto
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