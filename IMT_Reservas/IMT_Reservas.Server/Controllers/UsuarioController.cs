using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
        => _usuarioService = usuarioService;

    [HttpGet("{carnet}")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorCarnet(string carnet)
    {
        var usuarioDto = await _usuarioService.ObtenerUsuarioPorCarnetAsync(carnet);
        if (usuarioDto is null)
            return NotFound(new { Message = "Usuario no encontrado" });

        return Ok(usuarioDto);
    }

    [HttpPost("cerrar-sesion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult CerrarSesion()
    {
        // TODO: Implementar lógica de cierre de sesion
        return NoContent();
    }

    [HttpPost("crear-cuenta")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearCuenta([FromBody] UsuarioCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = new Usuario(
            carnet: dto.CarnetIdentidad,
            nombre: dto.Nombre,
            apellidoPaterno: dto.ApellidoPaterno,
            apellidoMaterno: dto.ApellidoMaterno,
            rol: Enum.TryParse<TipoUsuario>(dto.TipoUsuario, out var rol) ? rol : TipoUsuario.Estudiante,
            carreraId: int.TryParse(dto.Carrera, out var idCarrera) ? idCarrera : 0,
            contrasena: dto.Password,
            email: dto.Email,
            telefono: dto.Telefono,
            nombreReferencia: dto.NombreReferencia,
            telefonoReferencia: dto.TelefonoReferencia,
            emailReferencia: dto.EmailReferencia
        );

        await _usuarioService.CrearUsuarioAsync(usuario);

        var usuarioDto = new UsuarioReadDto
        {
            CarnetIdentidad = usuario.Carnet,
            Nombre = usuario.Nombre,
            ApellidoPaterno = usuario.ApellidoPaterno,
            ApellidoMaterno = usuario.ApellidoMaterno,
            Rol = usuario.Rol.ToString(),
            IdCarrera = usuario.CarreraId,
            Email = usuario.Email,
            Telefono = usuario.Telefono,
            NombreReferencia = usuario.NombreReferencia,
            TelefonoReferencia = usuario.TelefonoReferencia,
            EmailReferencia = usuario.EmailReferencia
        };

        return CreatedAtAction(
            nameof(ObtenerPorCarnet),
            new { carnet = usuario.Carnet },
            usuarioDto
        );
    }
}
