using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly PrestamoService servicio;
    public PrestamoController(PrestamoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromForm] CrearPrestamoComando input)
    {
        return Ok(servicio.Crear(input));
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpGet("historial")]
    public IActionResult ObtenerPorCarnetYEstado([FromQuery] string carnetUsuario, [FromQuery] string estadoPrestamo)
    {
        return Ok(servicio.ObtenerPrestamosPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarPrestamoComando(id)); return Ok(new { mensaje = "Préstamo eliminado exitosamente" });
    }

    [HttpPut("estadoPrestamo")]
    public IActionResult ActualizarEstado([FromBody] ActualizarEstadoPrestamoComando input)
    {
        servicio.ActualizarEstadoPrestamo(input); return Ok(new { mensaje = "Estado del préstamo actualizado exitosamente" });
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var prestamos = servicio.ObtenerTodos(); var prestamo = prestamos?.FirstOrDefault(p => p.Id == id); if (prestamo == null) return NotFound(); return Ok(prestamo);
    }

    [HttpGet("contrato/{prestamoId}")]
    public IActionResult ObtenerChunksContratoPorPrestamo(int prestamoId)
    {
        var consulta = new ObtenerContratoPorPrestamoConsulta(prestamoId);
        var chunks = servicio.ObtenerContratoPorPrestamo(consulta);
        return Ok(chunks);
    }
}