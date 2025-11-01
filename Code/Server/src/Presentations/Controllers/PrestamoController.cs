using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly IPrestamoService servicio;
    public PrestamoController(IPrestamoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromForm] CrearPrestamoComando input)
    {
        try { return Ok(servicio.CrearPrestamo(input)); }
        catch (ErrorCarnetUsuarioNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNoEquiposDisponibles ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (DomainException ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (System.Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosPrestamos()); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("historial")]
    public IActionResult ObtenerPorCarnetYEstado([FromQuery] string carnetUsuario, [FromQuery] string estadoPrestamo)
    {
        try { return Ok(servicio.ObtenerPrestamosPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo)); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarPrestamo(new EliminarPrestamoComando(id)); return Ok(new { mensaje = "Préstamo eliminado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (DomainException ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut("estadoPrestamo")]
    public IActionResult ActualizarEstado([FromBody] ActualizarEstadoPrestamoComando input)
    {
        try { servicio.ActualizarEstadoPrestamo(input); return Ok(new { mensaje = "Estado del préstamo actualizado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (DomainException ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        try { var prestamos = servicio.ObtenerTodosPrestamos(); var prestamo = prestamos?.FirstOrDefault(p => p.Id == id); if (prestamo == null) return NotFound(); return Ok(prestamo); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("contrato/{prestamoId}")]
    public IActionResult ObtenerChunksContratoPorPrestamo(int prestamoId)
    {
        try
        {
            var consulta = new ObtenerContratoPorPrestamoConsulta(prestamoId);
            var chunks = servicio.ObtenerContratoPorPrestamo(consulta);
            return Ok(chunks);
        }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}