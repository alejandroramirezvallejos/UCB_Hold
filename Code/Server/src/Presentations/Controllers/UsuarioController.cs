using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService servicio;
    public UsuarioController(IUsuarioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearUsuarioComando comando)
    {
        try { servicio.CrearUsuario(comando); return Ok(new { mensaje = "Usuario creado exitosamente" }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosUsuarios()); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarUsuarioComando comando)
    {
        try { servicio.ActualizarUsuario(comando); return Ok(new { mensaje = "Usuario actualizado exitosamente" }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{carnet}")]
    public IActionResult Eliminar(string carnet)
    {
        try { servicio.EliminarUsuario(new EliminarUsuarioComando(carnet)); return Ok(new { mensaje = "Usuario eliminado exitosamente" }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("{carnet}")]
    public IActionResult ObtenerPorCarnet(string carnet)
    {
        try { var usuarios = servicio.ObtenerTodosUsuarios(); var usuario = usuarios?.FirstOrDefault(u => u.Carnet == carnet); if (usuario == null) return NotFound(); return Ok(usuario); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("iniciarSesion")]
    public IActionResult IniciarSesion([FromQuery] string email, [FromQuery] string contrasena)
    {
        try { var consulta = new IniciarSesionUsuarioConsulta(email, contrasena); var usuario = servicio.IniciarSesionUsuario(consulta); return Ok(usuario); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}