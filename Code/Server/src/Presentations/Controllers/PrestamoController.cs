using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using IMT_Reservas.Server.Application.ResponseDTOs;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class PrestamoController : ControllerBase
{
    private readonly IPrestamoService _servicio;
    public PrestamoController(IPrestamoService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<PrestamoConEquiposDto> Crear([FromForm] CrearPrestamoComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<PrestamoDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpGet("historial")]
    public IActionResult ObtenerPorCarnetYEstado([FromQuery] string carnetUsuario, [FromQuery] string estadoPrestamo)
    {
        return Ok(_servicio.ObtenerPrestamosPorCarnetYEstadoPrestamo(carnetUsuario, estadoPrestamo));
    }

    [HttpDelete("{id}")]
    public Result<PrestamoDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarPrestamoComando(id));
    }

    [HttpPut("estadoPrestamo")]
    public IActionResult ActualizarEstado([FromBody] ActualizarEstadoPrestamoComando input)
    {
        _servicio.ActualizarEstadoPrestamo(input);
        return Ok(new { mensaje = "Estado del préstamo actualizado exitosamente" });
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var prestamos = _servicio.ObtenerTodos();
        var prestamo = prestamos?.Value?.FirstOrDefault(p => p.Id == id);
        if (prestamo == null) return NotFound();
        return Ok(prestamo);
    }

    [HttpGet("contrato/{prestamoId}")]
    public IActionResult ObtenerChunksContratoPorPrestamo(int prestamoId)
    {
        var consulta = new ObtenerContratoPorPrestamoConsulta(prestamoId);
        var chunks = _servicio.ObtenerContratoPorPrestamo(consulta);
        return Ok(chunks);
    }
}
