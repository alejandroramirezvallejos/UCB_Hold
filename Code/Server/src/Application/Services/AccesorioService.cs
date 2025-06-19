using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class AccesorioService : IAccesorioService
{
    private readonly AccesorioRepository _accesorioRepository;
    public AccesorioService(AccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }    
    public void CrearAccesorio(CrearAccesorioComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _accesorioRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorModeloRequerido)
        {
            throw; 
        }
        catch (ErrorLongitudInvalida)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorCodigoImtRequerido)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorValorNegativo)
        {
            throw; // Re-lanzar excepciones de validación
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_accesorios
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Errores específicos del procedimiento insertar_accesorios
                if (mensaje.Contains("no se encontró el equipo con código imt"))
                {
                    throw new ErrorReferenciaInvalida("El código IMT del equipo no existe o está inactivo");
                }
                
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un accesorio con esos datos"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                if (mensaje.Contains("error al insertar accesorio"))
                {
                    throw new Exception($"Error al crear accesorio: {errorDb.Message}", errorDb);
                }
                
                throw new Exception($"Error de base de datos al crear accesorio: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear accesorio: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    private void ValidarEntradaCreacion(CrearAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();
        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();
        if (comando.Nombre.Length > 256)
            throw new ErrorLongitudInvalida("nombre del accesorio", 256);
        if (comando.CodigoIMT <= 0)
            throw new ErrorCodigoImtRequerido();
        if (comando.Precio.HasValue && comando.Precio.Value <= 0)
            throw new ErrorValorNegativo("precio");
    }
    public List<AccesorioDto>? ObtenerTodosAccesorios()
    {
        try
        {
            DataTable dt = _accesorioRepository.ObtenerTodos();
            var lista = new List<AccesorioDto>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                lista.Add(MapearFilaADto(row));
            return lista;
        }
        catch
        {
            throw;
        }
    }    public void ActualizarAccesorio(ActualizarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _accesorioRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorModeloRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (ErrorCodigoImtRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_accesorio
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Errores específicos del procedimiento actualizar_accesorio
                if (mensaje.Contains("no se encontró un accesorio activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                if (mensaje.Contains("no se encontró un equipo activo con código imt"))
                {
                    throw new ErrorReferenciaInvalida("El código IMT del equipo no existe o está inactivo");
                }
                
                if (errorDb.SqlState == "23505" || mensaje.Contains("error de violación de unicidad"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                if (mensaje.Contains("error inesperado al actualizar el accesorio"))
                {
                    throw new Exception($"Error inesperado al actualizar el accesorio: {errorDb.Message}", errorDb);
                }
                
                throw new Exception($"Error de base de datos al actualizar accesorio: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar accesorio: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }public void EliminarAccesorio(EliminarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _accesorioRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_accesorio
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Errores específicos del procedimiento eliminar_accesorio
                if (mensaje.Contains("no se encontró un accesorio activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                if (mensaje.Contains("error al eliminar lógicamente el accesorio"))
                {
                    throw new Exception($"Error al eliminar accesorio: {errorDb.Message}", errorDb);
                }
                
                throw new Exception($"Error de base de datos al eliminar accesorio: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar accesorio: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }
    private static AccesorioDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id = Convert.ToInt32(fila["id_accesorio"]),
            Nombre = fila["nombre_accesorio"] == DBNull.Value ? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
            UrlDataSheet = fila["url_data_sheet_accesorio"] == DBNull.Value ? null : fila["url_data_sheet_accesorio"].ToString(),
        };
    }

    private void ValidarEntradaActualizacion(ActualizarAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255)
            throw new ErrorLongitudInvalida("nombre del accesorio", 255);

        if (comando.CodigoIMT <= 0)
            throw new ErrorCodigoImtRequerido();

        if (comando.Precio.HasValue && comando.Precio.Value < 0)
            throw new ErrorValorNegativo("precio");
    }

    private void ValidarEntradaEliminacion(EliminarAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
}