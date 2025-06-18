using System.Data;
public class MantenimientoService : IMantenimientoService
{
    private readonly MantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(MantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }    public void CrearMantenimiento(CrearMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _mantenimientoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (ErrorReferenciaInvalida)
        {
            throw;
        }
        catch (ErrorFechaInvalida)
        {
            throw;
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_mantenimiento según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Empresa de mantenimiento no encontrada
                if (mensaje.Contains("no se encontró la empresa con nombre"))
                {
                    throw new ErrorReferenciaInvalida("empresa de mantenimiento");
                }
                
                // Error: Arreglos con longitudes diferentes
                if (mensaje.Contains("los arreglos de equipos deben tener la misma longitud"))
                {
                    throw new ArgumentException("Los arreglos de equipos, tipos de mantenimiento y descripciones deben tener la misma longitud.");
                }
                
                // Error: Equipo no encontrado por código IMT
                if (mensaje.Contains("equipo no encontrado con código imt"))
                {
                    throw new ErrorReferenciaInvalida("equipo");
                }
                
                // Error: Violación de unicidad
                if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al insertar mantenimiento"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar mantenimiento"))
                {
                    throw new Exception($"Error inesperado al insertar mantenimiento: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear mantenimiento: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear mantenimiento: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }
    public void EliminarMantenimiento(EliminarMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _mantenimientoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_mantenimiento según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Mantenimiento no encontrado
                if (mensaje.Contains("no se encontró un mantenimiento activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el mantenimiento"))
                {
                    throw new Exception($"Error inesperado al eliminar mantenimiento: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar mantenimiento: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar mantenimiento: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }
    public List<MantenimientoDto>? ObtenerTodosMantenimientos()
    {
        try
        {
            DataTable resultado = _mantenimientoRepository.ObtenerTodos();
            var lista = new List<MantenimientoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    private MantenimientoDto MapearFilaADto(DataRow fila)
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
    }    private void ValidarEntradaCreacion(CrearMantenimientoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.FechaFinalDeMantenimiento < comando.FechaMantenimiento)
            throw new ErrorFechaInvalida();

        if (string.IsNullOrWhiteSpace(comando.NombreEmpresaMantenimiento))
            throw new ErrorNombreRequerido();

        if (comando.CodigoIMT == null || comando.CodigoIMT.Length == 0)
            throw new ErrorReferenciaInvalida("equipos");

        if (comando.TipoMantenimiento == null || comando.TipoMantenimiento.Length == 0)
            throw new ErrorReferenciaInvalida("tipos de mantenimiento");

        if (comando.CodigoIMT.Length != comando.TipoMantenimiento.Length)
            throw new ErrorReferenciaInvalida("equipos y tipos");

        if (comando.DescripcionEquipo != null && comando.DescripcionEquipo.Length > 0 &&
            comando.DescripcionEquipo.Length != comando.CodigoIMT.Length)
            throw new ErrorReferenciaInvalida("descripciones");

        if (comando.CodigoIMT.Any(codigo => codigo <= 0))
            throw new ErrorIdInvalido();

        if (comando.Costo.HasValue && comando.Costo.Value < 0)
            throw new ErrorValorNegativo("costo");
    }

    private void ValidarEntradaEliminacion(EliminarMantenimientoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
}