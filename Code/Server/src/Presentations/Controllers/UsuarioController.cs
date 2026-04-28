using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService servicio;
    public UsuarioController(UsuarioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearUsuarioComando comando)
    {
        servicio.Crear(comando); return Ok(new { mensaje = "Usuario creado exitosamente" });
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarUsuarioComando comando)
    {
        servicio.Actualizar(comando); return Ok(new { mensaje = "Usuario actualizado exitosamente" });
    }

    [HttpDelete("{carnet}")]
    public IActionResult Eliminar(string carnet)
    {
        servicio.Eliminar(new EliminarUsuarioComando(carnet)); return Ok(new { mensaje = "Usuario eliminado exitosamente" });
    }

    [HttpGet("{carnet}")]
    public IActionResult ObtenerPorCarnet(string carnet)
    {
        var usuarios = servicio.ObtenerTodos(); var usuario = usuarios?.FirstOrDefault(u => u.Carnet == carnet); if (usuario == null) return NotFound(); return Ok(usuario);
    }

    [HttpPost("iniciarSesion")]
    public IActionResult IniciarSesion([FromBody] IniciarSesionUsuarioConsulta consulta)
    {
        var usuario = servicio.IniciarSesionUsuario(consulta); return Ok(usuario);
    }
}