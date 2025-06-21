using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _servicio;

    public UsuarioController(IUsuarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] CrearUsuarioComando comando)
    {
        try
        {
            _servicio.CrearUsuario(comando);
            return Ok(new { message = "Usuario creado exitosamente" });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCarnetRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorApellidoPaternoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorApellidoMaternoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorContrasenaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCarreraRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorTelefonoRequerido ex)
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
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
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
            var usuarios = _servicio.ObtenerTodosUsuarios();

            if (usuarios == null || !usuarios.Any())
            {
                return Ok(new List<UsuarioDto>());
            }
            return Ok(usuarios);
        }
        catch (Exception ex) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener los usuarios" });
        }
    }
    [HttpPut]
    public IActionResult ActualizarUsuario([FromBody] ActualizarUsuarioComando comando)
    {
        try
        {
            _servicio.ActualizarUsuario(comando);
            return Ok(new { message = "Usuario actualizado exitosamente" });
        }
        catch (ErrorCarnetInvalido ex)
        {
            return BadRequest(new { error = "Carnet inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCarnetRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorApellidoPaternoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorApellidoMaternoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorContrasenaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCarreraRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorTelefonoRequerido ex)
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
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el usuario" });
        }
    }
    [HttpDelete("{carnet}")]
    public IActionResult EliminarUsuario(string carnet)
    {
        try
        {
            var comando = new EliminarUsuarioComando(carnet);
            _servicio.EliminarUsuario(comando);
            return Ok(new { message = "Usuario eliminado exitosamente" });
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
        catch (ErrorRegistroEnUso ex)
        {
            return Conflict(new { error = "Registro en uso", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
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
            var usuarios = _servicio.ObtenerTodosUsuarios();
            var usuario = usuarios?.FirstOrDefault(u => u.Carnet == carnet);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(usuario);
        }
        catch (Exception ex) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener el usuario" });
        }
    }
    [HttpGet("iniciarSesion")]
    public IActionResult IniciarSesion([FromQuery] string email, [FromQuery] string contrasena)
    {
        try
        {
            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            var usuario = _servicio.IniciarSesionUsuario(consulta);
            return Ok(usuario);
        }
        catch (Exception ex) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al iniciar sesión" });
        }
    }
}