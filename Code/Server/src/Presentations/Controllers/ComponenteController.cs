using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly ComponenteService servicio;
    public ComponenteController(ComponenteService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearComponenteComando input)
    {
        servicio.Crear(input); return Created("", new { message = "Componente creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarComponenteComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Componente actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarComponenteComando(id)); return NoContent();
    }
}