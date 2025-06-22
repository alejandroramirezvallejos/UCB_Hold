using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly IEquipoService servicio;
    public EquipoController(IEquipoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearEquipoComando input)
    {
        try { servicio.CrearEquipo(input); return Created(); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorModeloRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosEquipos()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarEquipoComando input)
    {
        try { servicio.ActualizarEquipo(input); return Ok(new { mensaje = "Equipo actualizado exitosamente" }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (NotFoundException ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ConflictException ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (DomainException ex) when (!(ex is ErrorRegistroNoEncontrado) && !(ex is ErrorRegistroYaExiste))
        {
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarEquipo(new EliminarEquipoComando(id)); return NoContent(); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}