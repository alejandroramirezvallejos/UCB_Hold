using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class EquipoService : IEquipoService
{
    private readonly EquipoRepository _equipoRepository;

    public EquipoService(EquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }    public virtual void CrearEquipo(CrearEquipoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _equipoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Grupo de equipos no encontrado
                if (mensaje.Contains("no se encontró el grupo de equipos con nombre"))
                {
                    throw new ErrorReferenciaInvalida("nombre grupo de equipos");
                }
                
                // Error: Gavetero no encontrado
                if (mensaje.Contains("no se encontro el gavetero con nombre"))
                {
                    throw new ErrorReferenciaInvalida("nombre gavetero");
                }
                
                // Error: Equipo duplicado (código UCB o número serial)
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un equipo con ese código ucb o número serial"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar equipo"))
                {
                    throw new Exception($"Error inesperado al insertar equipo: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear equipo: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear equipo: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    private void ValidarEntradaCreacion(CrearEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.NombreGrupoEquipo))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();

        if (string.IsNullOrWhiteSpace(comando.Marca))
            throw new ErrorMarcaRequerida();

        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0)
            throw new ErrorValorNegativo("costo de referencia");

        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0)
            throw new ErrorValorNegativo("Tiempo máximo de préstamo");
    }    public virtual void ActualizarEquipo(ActualizarEquipoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _equipoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Equipo no encontrado
                if (mensaje.Contains("no se encontró un equipo activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error: Grupo de equipos no encontrado
                if (mensaje.Contains("no se encontró el grupo de equipos con nombre"))
                {
                    throw new ErrorReferenciaInvalida("grupo de equipos");
                }
                
                // Error: Gavetero no encontrado
                if (mensaje.Contains("no se encontró el gavetero con nombre"))
                {
                    throw new ErrorReferenciaInvalida("gavetero");
                }
                
                // Error: Estado de equipo inválido
                if (mensaje.Contains("valor inválido para estado_equipo"))
                {
                    throw new ArgumentException("Estado de equipo inválido. Debe ser 'operativo', 'inoperativo', o 'parcialmente_operativo'.");
                }
                
                // Errores de violación de unicidad específicos
                if (errorDb.SqlState == "23505")
                {
                    if (mensaje.Contains("código imt") || mensaje.Contains("unique_codigo_imt"))
                    {
                        throw new ErrorRegistroYaExiste();
                    }
                    else if (mensaje.Contains("código ucb") || mensaje.Contains("unique_codigo_ucb"))
                    {
                        throw new ErrorRegistroYaExiste();
                    }
                    else if (mensaje.Contains("número de serie") || mensaje.Contains("unique_numero_serial"))
                    {
                        throw new ErrorRegistroYaExiste();
                    }
                    else
                    {
                        throw new ErrorRegistroYaExiste();
                    }
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error inesperado al actualizar el equipo"))
                {
                    throw new Exception($"Error inesperado al actualizar equipo: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al actualizar equipo: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar equipo: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    private void ValidarEntradaActualizacion(ActualizarEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0)
            throw new ErrorValorNegativo("costo de referencia");

        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0)
            throw new ErrorValorNegativo("Tiempo máximo de préstamo");
    }

    public virtual void EliminarEquipo(EliminarEquipoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _equipoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Equipo no encontrado
                if (mensaje.Contains("no se encontró un equipo activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el equipo"))
                {
                    throw new Exception($"Error inesperado al eliminar equipo: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar equipo: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar equipo: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    private void ValidarEntradaEliminacion(EliminarEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
    public virtual List<EquipoDto>? ObtenerTodosEquipos()
    {
        try
        {
            DataTable resultado = _equipoRepository.ObtenerTodos();
            var lista = new List<EquipoDto>(resultado.Rows.Count);
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
    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
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
}