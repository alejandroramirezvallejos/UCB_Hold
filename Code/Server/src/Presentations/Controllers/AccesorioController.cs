using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly AccesorioService servicio;
    public AccesorioController(AccesorioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearAccesorioComando input)
    {
        servicio.Crear(input); return Created("", new { message = "Accesorio creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarAccesorioComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Accesorio actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarAccesorioComando(id)); return NoContent();
    }
}