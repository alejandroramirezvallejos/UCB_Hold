using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService servicio;

    public UsuarioController(UsuarioService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] CrearUsuarioComando input)
    {
        try
        {
            servicio.CrearUsuario(input);
            return Ok(new { mensaje = "Usuario creado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict($"Usuario ya existe: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult ObtenerUsuarios()
    {
        try
        {
            var usuarios = servicio.ObtenerTodosUsuarios();

            if (usuarios == null || !usuarios.Any())
            {
                return Ok(new List<UsuarioDto>());
            }

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }    [HttpPut("{carnet}")]
    public IActionResult ActualizarUsuario([FromBody] ActualizarUsuarioComando input)
    {
        try
        {
            servicio.ActualizarUsuario(input);
            return Ok(new { mensaje = "Usuario actualizado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound($"Usuario no encontrado: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{carnet}")]
    public IActionResult EliminarUsuario(string carnet)
    {
        try
        {
            var comando = new EliminarUsuarioComando(carnet);
            servicio.EliminarUsuario(comando);
            return Ok(new { mensaje = "Usuario eliminado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound($"Usuario no encontrado: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{carnet}")]
    public IActionResult ObtenerUsuarioPorCarnet(string carnet)
    {
        try
        {
            var usuarios = servicio.ObtenerTodosUsuarios();
            var usuario = usuarios?.FirstOrDefault(u => u.Carnet == carnet);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
    [HttpGet("iniciarSesion")]
    public IActionResult IniciarSesion([FromQuery] string email,[FromQuery] string contrasena)
    {
        try
        {
            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            var usuario = servicio.IniciarSesionUsuario(consulta);
            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}