using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICrearUsuarioComando      _crearUsuarioComando;
    private readonly IObtenerUsuarioConsulta   _obtenerUsuarioConsulta;
    private readonly IActualizarUsuarioComando _actualizarUsuarioComando;
    private readonly IEliminarUsuarioComando   _eliminarUsuarioComando;
    private readonly IIniciarSesionUsuarioConsulta _iniciarSesionUsuarioConsulta;

    public UsuarioController(
        ICrearUsuarioComando crearUsuarioComando,
        IObtenerUsuarioConsulta obtenerUsuarioConsulta,
        IActualizarUsuarioComando actualizarUsuarioComando,
        IEliminarUsuarioComando eliminarUsuarioComando,
        IIniciarSesionUsuarioConsulta iniciarSesionUsuarioConsulta)
    {
        _crearUsuarioComando = crearUsuarioComando;
        _obtenerUsuarioConsulta = obtenerUsuarioConsulta;
        _actualizarUsuarioComando = actualizarUsuarioComando;
        _eliminarUsuarioComando = eliminarUsuarioComando;
        _iniciarSesionUsuarioConsulta = iniciarSesionUsuarioConsulta;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] UsuarioRequestDto dto)
    {
        try
        {
            
            if (string.IsNullOrWhiteSpace(dto.Carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }
            if (dto.Nombre == null)
            {
                return BadRequest("El nombre es obligatorio");
            }
            if(dto.ApellidoPaterno==null){
                return BadRequest("El apellido paterno es obligatorio");
            }
            if(dto.ApellidoMaterno==null){
                return BadRequest("El apellido materno es obligatorio");
            }
            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
            {
                return BadRequest("El email es obligatorio");
            }
            if (string.IsNullOrWhiteSpace(dto.Contrasena) )
            {
                return BadRequest("La contraseña es obligatoria");
            }
            if (dto.NombreCarrera == null)
            {
                return BadRequest("El nombre de la carrera es obligatorio");
            }
            if (dto.Telefono == null)
            {
                return BadRequest("El teléfono es obligatorio");
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
    }    [HttpPut("{carnet}")]
    public IActionResult ActualizarUsuario(string carnet, [FromBody] ActualizarUsuarioRequestDto dto)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(carnet))
            {
                return BadRequest("El carnet es obligatorio");
            }


            var comando = new ActualizarUsuarioComando(
                carnet,
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
    [HttpGet("iniciarSesion")]
    public IActionResult IniciarSesion([FromQuery] string email,[FromQuery] string contrasena)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var consulta = new IniciarSesionUsuarioConsulta(email, contrasena);
            var usuario = _iniciarSesionUsuarioConsulta.Handle(consulta);

            if (usuario == null)
            {
                return Unauthorized("Email o contraseña incorrectos");
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