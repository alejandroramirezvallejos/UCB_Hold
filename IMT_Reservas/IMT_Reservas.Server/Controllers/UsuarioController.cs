using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("usuario")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
    }

    [HttpGet("{carnet}")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObtenerPorCarnet(string carnet)
    {
        if (string.IsNullOrWhiteSpace(carnet))
            return BadRequest(new { Message = "El carnet no puede ser nulo o vacío." });

        var usuarioDto = _usuarioService.ObtenerUsuarioPorCarnet(carnet);
        if (usuarioDto is null)
            return NotFound(new { Message = "Usuario no encontrado" });

        return Ok(usuarioDto);
    }

    [HttpPost("cerrar-sesion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult CerrarSesion()
    {
        // TODO: Implementar lógica de cierre de sesión
        return NoContent();
    }

    [HttpPost("crear-cuenta")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CrearCuenta([FromBody] UsuarioCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!Enum.TryParse<TipoUsuario>(dto.TipoUsuario, true, out var rol))
            return BadRequest($"Tipo de usuario inválido: '{dto.TipoUsuario}'");

        var nombreCarrera = dto.Carrera?.Trim() ?? string.Empty;
        Usuario usuario;
        try
        {
            usuario = new Usuario(
                carnet:             dto.CarnetIdentidad,
                nombre:             dto.Nombre,
                apellidoPaterno:    dto.ApellidoPaterno,
                apellidoMaterno:    dto.ApellidoMaterno,
                rol:                rol,
                nombreCarrera:      nombreCarrera,
                contrasena:         dto.Password,
                email:              dto.Email,
                telefono:           dto.Telefono,
                nombreReferencia:   dto.NombreReferencia,
                telefonoReferencia: dto.TelefonoReferencia,
                emailReferencia:    dto.EmailReferencia
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }

        _usuarioService.CrearUsuario(usuario);

        var usuarioDto = new UsuarioReadDto
        {
            CarnetIdentidad    = usuario.Carnet,
            Nombre             = usuario.Nombre,
            ApellidoPaterno    = usuario.ApellidoPaterno,
            ApellidoMaterno    = usuario.ApellidoMaterno,
            Rol                = usuario.Rol.ToString(),
            NombreCarrera      = usuario.NombreCarrera,
            Email              = usuario.Email,
            Telefono           = usuario.Telefono,
            NombreReferencia   = usuario.NombreReferencia,
            TelefonoReferencia = usuario.TelefonoReferencia,
            EmailReferencia    = usuario.EmailReferencia
        };

        return CreatedAtAction(
            nameof(ObtenerPorCarnet),
            new { carnet = usuario.Carnet },
            usuarioDto
        );
    }
}
