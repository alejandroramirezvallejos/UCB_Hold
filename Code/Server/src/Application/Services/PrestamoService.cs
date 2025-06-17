using System.Data;
using System.Linq;
using Shared.Common;

public class PrestamoService : IPrestamoService
{
    private readonly PrestamoRepository _prestamoRepository;

    public PrestamoService(PrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }    public void CrearPrestamo(CrearPrestamoComando comando)
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
        catch (ErrorCarnetUsuarioNoEncontrado)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (ErrorNoEquiposDisponibles)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (Exception ex)
        {
            // 3. Interpretar errores específicos del procedimiento insertar_prestamo
            throw InterpretarErrorCreacionPrestamo(ex, comando);
        }
    }

    private void ValidarEntradaCreacion(CrearPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario))
            throw new ErrorNombreRequerido();

        if (comando.GrupoEquipoId == null || comando.GrupoEquipoId.Length == 0)
            throw new ErrorIdInvalido();

        if (comando.GrupoEquipoId.Any(id => id <= 0))
            throw new ErrorIdInvalido();

        if (comando.FechaPrestamoEsperada < DateTime.Now.Date)
            throw new ArgumentException("La fecha de préstamo no puede ser anterior a hoy");

        if (comando.FechaDevolucionEsperada <= comando.FechaPrestamoEsperada)
            throw new ArgumentException("La fecha de devolución debe ser posterior a la fecha de préstamo");
    }

    private Exception InterpretarErrorCreacionPrestamo(Exception ex, CrearPrestamoComando comando)
    {
        var errorMessage = ex.Message.ToLower();
        
        // Errores específicos del procedimiento insertar_prestamo
          // Error: Usuario no encontrado (Foreign Key en carnet)
        if (errorMessage.Contains("23503") && (errorMessage.Contains("carnet") || errorMessage.Contains("usuarios")))
        {
            return new ErrorCarnetUsuarioNoEncontrado();
        }
        
        // Error: Grupo no existe o está eliminado
        if (errorMessage.Contains("grupo id") && errorMessage.Contains("no existe"))
        {
            var match = System.Text.RegularExpressions.Regex.Match(errorMessage, @"grupo id (\d+) no existe");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int grupoId))
            {                return new ErrorRegistroNoEncontrado();
            }
            return new ErrorRegistroNoEncontrado();
        }
        
        // Error: No equipo disponible para grupo
        if (errorMessage.Contains("no se encontró equipo disponible") || errorMessage.Contains("para el grupo id"))
        {
            var match = System.Text.RegularExpressions.Regex.Match(errorMessage, @"grupo id (\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int grupoId))
            {
                return new ErrorNoEquiposDisponibles();
            }
            return new ErrorNoEquiposDisponibles();
        }
        
        // Error: Fallo crítico al crear préstamo
        if (errorMessage.Contains("fallo crítico") || errorMessage.Contains("no se pudo obtener el id"))
        {
            return new Exception("Fallo crítico: No se pudo crear el préstamo");
        }
        
        // Usar intérprete genérico como fallback
        var parametros = new Dictionary<string, object?>
        {
            ["carnetUsuario"] = comando.CarnetUsuario,
            ["fechaInicio"] = comando.FechaPrestamoEsperada,
            ["fechaFin"] = comando.FechaDevolucionEsperada
        };        
        return PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "préstamo", parametros);
    }

    public void EliminarPrestamo(EliminarPrestamoComando comando)
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
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "préstamo", parametros);
        }
    }

    private void ValidarEntradaEliminacion(EliminarPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }

    public List<PrestamoDto>? ObtenerTodosPrestamos()
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