using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly GaveteroService servicio;
    public GaveteroController(GaveteroService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearGaveteroComando input)
    {
        servicio.Crear(input); 
        return Created(); 
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGaveteroComando input)
    {
        servicio.Actualizar(input); 
        return Ok(new { mensaje = "Gavetero actualizado exitosamente" }); 
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarGaveteroComando(id)); 
        return NoContent(); 
    }
}