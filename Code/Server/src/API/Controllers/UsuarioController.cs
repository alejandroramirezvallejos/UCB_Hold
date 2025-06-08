using API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICrearUsuarioComando _crearUsuarioComando;
    private readonly IObtenerUsuarioConsulta _obtenerUsuarioConsulta;
    private readonly IActualizarUsuarioComando _actualizarUsuarioComando;
    private readonly IEliminarUsuarioComando _eliminarUsuarioComando;

    public UsuarioController(
        ICrearUsuarioComando crearUsuarioComando,
        IObtenerUsuarioConsulta obtenerUsuarioConsulta,
        IActualizarUsuarioComando actualizarUsuarioComando,
        IEliminarUsuarioComando eliminarUsuarioComando)
    {
        _crearUsuarioComando = crearUsuarioComando;
        _obtenerUsuarioConsulta = obtenerUsuarioConsulta;
        _actualizarUsuarioComando = actualizarUsuarioComando;
        _eliminarUsuarioComando = eliminarUsuarioComando;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] UsuarioRequestDto dto)
    {
        try
        {
            // Validaciones de ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validaciones adicionales de negocio
            if (string.IsNullOrWhiteSpace(dto.Carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }

            // Validación de formato de carnet (opcional - ajustar según reglas de negocio)
            if (dto.Carnet.Length < 3 || dto.Carnet.Length > 20)
            {
                return BadRequest("El carnet debe tener entre 3 y 20 caracteres");
            }

            // Validación de email único (opcional - depende de reglas de negocio)
            if (!IsValidEmail(dto.Email))
            {
                return BadRequest("El formato del email no es válido");
            }

            // Validación de teléfonos
            if (!string.IsNullOrEmpty(dto.TelefonoReferencia) && dto.TelefonoReferencia.Length < 7)
            {
                return BadRequest("El teléfono de referencia debe tener al menos 7 caracteres");
            }

            var comando = new CrearUsuarioComando(
                dto.Carnet,
                dto.Nombre,
                dto.ApellidoPaterno,
                dto.ApellidoMaterno,
                dto.Email,
                dto.Contrasena,
                dto.NombreCarrera,
                dto.Telefono,
                dto.TelefonoReferencia,
                dto.NombreReferencia,
                dto.EmailReferencia
            );

            _crearUsuarioComando.Handle(comando);
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
            var usuarios = _obtenerUsuarioConsulta.Handle();
            
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
    }

    [HttpPut("{carnet}")]
    public IActionResult ActualizarUsuario(string carnet, [FromBody] ActualizarUsuarioRequestDto dto)
    {
        try
        {
            // Validaciones de ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el carnet del parámetro coincida con el del DTO
            if (carnet != dto.Carnet)
            {
                return BadRequest("El carnet del parámetro no coincide con el del cuerpo de la solicitud");
            }

            // Validaciones adicionales
            if (string.IsNullOrWhiteSpace(carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }

            // Validar email si se proporciona
            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
            {
                return BadRequest("El formato del email no es válido");
            }

            // Validar teléfonos si se proporcionan
            if (!string.IsNullOrEmpty(dto.Telefono) && dto.Telefono.Length < 7)
            {
                return BadRequest("El teléfono debe tener al menos 7 caracteres");
            }

            if (!string.IsNullOrEmpty(dto.TelefonoReferencia) && dto.TelefonoReferencia.Length < 7)
            {
                return BadRequest("El teléfono de referencia debe tener al menos 7 caracteres");
            }

            var comando = new ActualizarUsuarioComando(
                dto.Carnet,
                dto.Nombre,
                dto.ApellidoPaterno,
                dto.ApellidoMaterno,
                dto.Email,
                dto.Contrasena,
                dto.Rol,
                dto.NombreCarrera,
                dto.Telefono,
                dto.TelefonoReferencia,
                dto.NombreReferencia,
                dto.EmailReferencia
            );

            _actualizarUsuarioComando.Handle(comando);
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
            if (string.IsNullOrWhiteSpace(carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }

            // Validación de formato de carnet
            if (carnet.Length < 3 || carnet.Length > 20)
            {
                return BadRequest("El carnet debe tener entre 3 y 20 caracteres");
            }

            var comando = new EliminarUsuarioComando(carnet);
            _eliminarUsuarioComando.Handle(comando);
            
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
            if (string.IsNullOrWhiteSpace(carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }

            var usuarios = _obtenerUsuarioConsulta.Handle();
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

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}