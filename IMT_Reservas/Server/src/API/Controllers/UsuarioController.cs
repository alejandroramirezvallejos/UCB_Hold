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
            return NotFound(new { Message = "Usuario no encontrado." });

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

        Usuario usuario;
        try
        {
            usuario = new Usuario(
                carnet:             dto.Carnet,
                nombre:             dto.Nombre,
                apellidoPaterno:    dto.ApellidoPaterno,
                apellidoMaterno:    dto.ApellidoMaterno,
                rol:                dto.Rol,
                carreraId:          dto.CarreraId,
                contrasena:         dto.Contrasena,
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

        var usuarioReadDto = new UsuarioReadDto
        {
            Carnet             = usuario.Carnet,
            Nombre             = usuario.Nombre,
            ApellidoPaterno    = usuario.ApellidoPaterno,
            ApellidoMaterno    = usuario.ApellidoMaterno,
            Rol                = usuario.Rol,
            CarreraId          = usuario.CarreraId,
            Email              = usuario.Email,
            Telefono           = usuario.Telefono,
            NombreReferencia   = usuario.NombreReferencia,
            TelefonoReferencia = usuario.TelefonoReferencia,
            EmailReferencia    = usuario.EmailReferencia,
            EstaEliminado      = usuario.EstaEliminado
        };

        return CreatedAtAction(
            nameof(ObtenerPorCarnet),
            new { carnet = usuario.Carnet },
            usuarioReadDto
        );
    }
}
