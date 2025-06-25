using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MantenimientoService : ServiciosAbstraccion, IMantenimientoService
{
    private readonly IMantenimientoRepository _mantenimientoRepository;
    public MantenimientoService(IMantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }
    public virtual void CrearMantenimiento(CrearMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _mantenimientoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (ErrorReferenciaInvalida) { throw; }
        catch (ErrorFechaInvalida) { throw; }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }    
    public override void ValidarEntradaCreacion<T>(T comando)
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
    
    public override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró la empresa con nombre")) throw new ErrorEmpresaMantenimientoNoEncontrada();
            if (mensaje.Contains("los arreglos de equipos deben tener la misma longitud")) throw new ErrorCodigoImtYTiposLongitudDiferente();
            if (mensaje.Contains("equipo no encontrado con código imt")) throw new ErrorCodigoImtNoEncontrado();
            if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al insertar mantenimiento")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar mantenimiento")) throw new Exception($"Error inesperado al insertar mantenimiento: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear mantenimiento: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear mantenimiento: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    public virtual void EliminarMantenimiento(EliminarMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _mantenimientoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }    
    public override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarMantenimientoComando
        if (comando is EliminarMantenimientoComando mantenimientoComando)
        {
            if (mantenimientoComando.Id <= 0) throw new ErrorIdInvalido("mantenimiento");
        }
    }
    
    public override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un mantenimiento activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el mantenimiento")) throw new Exception($"Error inesperado al eliminar mantenimiento: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar mantenimiento: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar mantenimiento: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en eliminación");
    }    public virtual List<MantenimientoDto>? ObtenerTodosMantenimientos()
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
    public override BaseDto MapearFilaADto(DataRow fila)
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