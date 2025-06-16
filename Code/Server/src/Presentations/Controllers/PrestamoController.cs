using API.ViewModels;
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
    }

    [HttpPost]
    public IActionResult CrearPrestamo([FromBody] CrearPrestamoComando input)
    {
        try
        {
            servicio.CrearPrestamo(input);
            return Ok(new { mensaje = "Préstamo creado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict($"Conflicto en la operación: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
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
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult EliminarPrestamo(int id)
    {
        try
        {
            var comando = new EliminarPrestamoComando(id);
            servicio.EliminarPrestamo(comando);

            return Ok(new { mensaje = "Préstamo eliminado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound($"Préstamo no encontrado: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}

