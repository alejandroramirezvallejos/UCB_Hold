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
    }    [HttpPost]
    public IActionResult CrearUsuario([FromBody] CrearUsuarioComando input)
    {
        try
        {
            servicio.CrearUsuario(input);
            return Ok(new { mensaje = "Usuario creado exitosamente" });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCarnetInvalido ex)
        {
            return BadRequest(new { error = "Carnet inválido", mensaje = ex.Message });
        }
        catch (ErrorEmailInvalido ex)
        {
            return BadRequest(new { error = "Email inválido", mensaje = ex.Message });
        }
        catch (ErrorCampoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Usuario duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el usuario" });
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
            }            return Ok(usuarios);
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener los usuarios" });
        }
    }    [HttpPut("{carnet}")]
    public IActionResult ActualizarUsuario([FromBody] ActualizarUsuarioComando input)
    {
        try
        {
            servicio.ActualizarUsuario(input);
            return Ok(new { mensaje = "Usuario actualizado exitosamente" });
        }
        catch (ErrorCarnetInvalido ex)
        {
            return BadRequest(new { error = "Carnet inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorEmailInvalido ex)
        {
            return BadRequest(new { error = "Email inválido", mensaje = ex.Message });
        }
        catch (ErrorCampoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Usuario no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Usuario duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el usuario" });
        }
    }    [HttpDelete("{carnet}")]
    public IActionResult EliminarUsuario(string carnet)
    {
        try
        {
            var comando = new EliminarUsuarioComando(carnet);
            servicio.EliminarUsuario(comando);
            return Ok(new { mensaje = "Usuario eliminado exitosamente" });
        }
        catch (ErrorCarnetInvalido ex)
        {
            return BadRequest(new { error = "Carnet inválido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Usuario no encontrado", mensaje = $"No se encontró un usuario con carnet '{carnet}'" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el usuario" });
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

            return Ok(usuario);        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener el usuario" });
        }
    }
    [HttpGet("iniciarSesion")]
    public IActionResult IniciarSesion([FromQuery] string email,[FromQuery] string contrasena)
    {
        try
        {
            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            var usuario = servicio.IniciarSesionUsuario(consulta);
            return Ok(usuario);        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al iniciar sesión" });
        }
    }
}