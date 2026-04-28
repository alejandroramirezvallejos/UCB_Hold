using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly GrupoEquipoService servicio;
    public GrupoEquipoController(GrupoEquipoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearGrupoEquipoComando input)
    {
        servicio.Crear(input); return Created("", new { mensaje = "Grupo de equipo creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorIdConsulta(id);
            var resultado = servicio.ObtenerGrupoEquipoPorId(consulta);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpGet("buscar")]
    public IActionResult ObtenerPorNombreYCategoria([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorNombreYCategoriaConsulta(nombre, categoria);
            var resultado = servicio.ObtenerGrupoEquipoPorNombreYCategoria(consulta);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGrupoEquipoComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Grupo de equipo actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarGrupoEquipoComando(id)); return NoContent();
    }
}
