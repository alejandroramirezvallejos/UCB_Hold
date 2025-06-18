using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly PrestamoService servicio;

    public PrestamoController(PrestamoService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult CrearPrestamo([FromBody] CrearPrestamoComando input)
    {
        try
        {
            servicio.CrearPrestamo(input);
            return Ok(new { mensaje = "Préstamo creado exitosamente" });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorCarnetUsuarioNoEncontrado ex)
        {
            return NotFound(new { error = "Usuario no encontrado", mensaje = ex.Message });
        }
        catch (ErrorNoEquiposDisponibles ex)
        {
            return Conflict(new { 
                error = "Equipos no disponibles", 
                mensaje = ex.Message
            });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Registro no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Préstamo duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "Argumentos inválidos", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el préstamo" });
        }
    }

    [HttpGet]
    public IActionResult ObtenerPrestamos()
    {
        try
        {
            var prestamos = servicio.ObtenerTodosPrestamos();

            if (prestamos == null || !prestamos.Any())
            {
                return Ok(new List<PrestamoDto>());
            }

            return Ok(prestamos);
        }        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener los préstamos" });
        }
    }    [HttpDelete("{id}")]
    public IActionResult EliminarPrestamo(int id)
    {
        try
        {
            var comando = new EliminarPrestamoComando(id);
            servicio.EliminarPrestamo(comando);
            return Ok(new { mensaje = "Préstamo eliminado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Préstamo no encontrado", mensaje = $"No se encontró un préstamo con ID {id}" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el préstamo" });
        }
    }
}

