using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly EquipoService servicio;
    public EquipoController(EquipoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearEquipoComando input)
    {
        servicio.Crear(input); return Created("", new { mensaje = "Equipo creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarEquipoComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Equipo actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarEquipoComando(id)); return NoContent();
    }
}