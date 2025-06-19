using System.Data;
using IMT_Reservas.Server.Shared.Common;


public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }    public virtual void CrearPrestamo(CrearPrestamoComando comando)
    {
        try
        {
            // 1. Validaciones de entrada
            ValidarEntradaCreacion(comando);
            
            // 2. Intentar crear en repository
            _prestamoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorIdInvalido)
        {
            throw; // Re-lanzar excepciones de validación  
        }
        catch (ErrorGrupoEquipoIdInvalido)
        {
            throw;
        }
        catch (ErrorFechaPrestamoYFechaDevolucionInvalidas)
        {
            throw;
        }
        catch (ErrorCarnetUsuarioNoEncontrado)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (ErrorNoEquiposDisponibles)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (Exception ex)
        {            // 3. Interpretar errores específicos del procedimiento insertar_prestamo
            InterpretarErrorCreacionPrestamo(ex, comando);
        }
    }

    private void ValidarEntradaCreacion(CrearPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario))
            throw new ErrorNombreRequerido();

        if (comando.GrupoEquipoId == null || comando.GrupoEquipoId.Length == 0)
            throw new ErrorGrupoEquipoIdInvalido();

        if (comando.GrupoEquipoId.Any(id => id <= 0))
            throw new ErrorGrupoEquipoIdInvalido();

        if (comando.FechaPrestamoEsperada == null)
            throw new ErrorFechaPrestamoEsperadaRequerida();

        if (comando.FechaDevolucionEsperada == null)
            throw new ErrorFechaDevolucionEsperadaRequerida();

        if (comando.FechaDevolucionEsperada < comando.FechaPrestamoEsperada)
            throw new ErrorFechaPrestamoYFechaDevolucionInvalidas();
    }

    private void InterpretarErrorCreacionPrestamo(Exception ex, CrearPrestamoComando comando)
    {
        var errorMessage = ex.Message?.ToLower() ?? "";
        
        // Errores específicos del procedimiento insertar_prestamo
        
        // 1. Errores de préstamo general
        if (errorMessage.Contains("error al crear préstamo general: conflicto de llave única"))
        {
            throw new ErrorRegistroYaExiste();
        }
        else if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear el préstamo general"))
        {
            throw new Exception($"Error inesperado al crear el préstamo general: {ex.Message}", ex);
        }
        else if (errorMessage.Contains("fallo crítico: no se pudo obtener el id del préstamo general creado"))
        {
            throw new Exception("Fallo crítico: No se pudo crear el préstamo", ex);
        }
        
        // 2. Errores de grupos
        else if (errorMessage.Contains("grupo id") && errorMessage.Contains("no existe o está eliminado"))
        {
            throw new ErrorRegistroNoEncontrado();
        }
        
        // 3. Errores de disponibilidad de equipos
        else if (errorMessage.Contains("no se encontró equipo disponible") && errorMessage.Contains("para el grupo id"))
        {
            throw new ErrorNoEquiposDisponibles();
        }
        
        // 4. Errores de detalles de préstamo
        else if (errorMessage.Contains("conflicto de llave única al crear detalle"))
        {
            throw new ErrorRegistroYaExiste();
        }
        else if (errorMessage.Contains("violación de restricción check al crear detalle"))
        {
            throw new Exception($"Error de validación al crear detalle del préstamo: {ex.Message}", ex);
        }
        else if (errorMessage.Contains("violación de llave foránea al crear detalle"))
        {
            throw new ErrorReferenciaInvalida("equipo o préstamo");
        }
        else if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear detalle"))
        {
            throw new Exception($"Error inesperado al crear detalle del préstamo: {ex.Message}", ex);
        }
        
        // 5. Errores por códigos SQLSTATE específicos
        else if (ex is ErrorDataBase errorDb)
        {
            // Violación de llave foránea (carnet de usuario no existe)
            if (errorDb.SqlState == "23503")
            {
                if (errorMessage.Contains("carnet") || errorMessage.Contains("usuarios"))
                {
                    throw new ErrorCarnetUsuarioNoEncontrado();
                }
                throw new ErrorReferenciaInvalida("referencia de base de datos");
            }
            
            // Violación de unicidad
            if (errorDb.SqlState == "23505")
            {
                throw new ErrorRegistroYaExiste();
            }
            
            throw new Exception($"Error inesperado de base de datos al crear préstamo: {errorDb.Message}", errorDb);
        }
        
        // 6. Errores de repositorio
        else if (ex is ErrorRepository errorRepo)
        {
            throw new Exception($"Error del repositorio al crear préstamo: {errorRepo.Message}", errorRepo);
        }
        
        // 7. Error genérico
        else
        {
            throw new Exception($"Error inesperado al crear préstamo: {ex.Message}", ex);
        }
    }

    public virtual void EliminarPrestamo(EliminarPrestamoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _prestamoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Manejo específico para eliminar_prestamo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";

                // Error: Préstamo no encontrado
                if (mensaje.Contains("no se encontró un préstamo activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }

                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el préstamo"))
                {
                    throw new Exception($"Error inesperado al eliminar préstamo: {errorDb.Message}", errorDb);
                }

                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar préstamo: {errorDb.Message}", errorDb);
            }

            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar préstamo: {errorRepo.Message}", errorRepo);
            }

            throw;
        }
    }

    private void ValidarEntradaEliminacion(EliminarPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }

    public virtual List<PrestamoDto>? ObtenerTodosPrestamos()
    {
        try
        {
            DataTable resultado = _prestamoRepository.ObtenerTodos();
            var lista = new List<PrestamoDto>(resultado.Rows.Count);
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

    private static PrestamoDto MapearFilaADto(DataRow fila)
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
        };
    }
}