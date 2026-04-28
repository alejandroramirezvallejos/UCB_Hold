using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly CarreraService _servicio;
    public CarreraController(CarreraService servicio) => _servicio = servicio;

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(_servicio.ObtenerTodos());
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CrearCarreraComando input)
    {
        _servicio.Crear(input); return Created($"api/carrera/{input.Nombre}", input);
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCarreraComando input)
    {
        _servicio.Actualizar(input); return Ok(new { mensaje = "Carrera actualizada exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        _servicio.Eliminar(new EliminarCarreraComando(id)); return NoContent();
    }
}