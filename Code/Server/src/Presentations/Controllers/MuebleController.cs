using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
    private readonly MuebleService servicio;
    public MuebleController(MuebleService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearMuebleComando input)
    {
        servicio.Crear(input); return Created("", new { mensaje = "Mueble creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarMuebleComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Mueble actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarMuebleComando(id)); return NoContent();
    }
}