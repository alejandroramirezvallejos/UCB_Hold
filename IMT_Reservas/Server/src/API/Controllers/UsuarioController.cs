using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICrearUsuarioComando      _crearUsuario;
    private readonly IObtenerUsuarioConsulta   _obtenerUsuario;
    private readonly IActualizarUsuarioComando _actualizarUsuario;
    private readonly IEliminarUsuarioComando   _eliminarUsuario;

    public UsuarioController(ICrearUsuarioComando crearUsuario, IObtenerUsuarioConsulta obtenerUsuario,
        IActualizarUsuarioComando actualizarUsuario, IEliminarUsuarioComando eliminarUsuario)
    {
        _crearUsuario      = crearUsuario;
        _obtenerUsuario    = obtenerUsuario;
        _actualizarUsuario = actualizarUsuario;
        _eliminarUsuario   = eliminarUsuario;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] UsuarioRequestDto solicitud)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CrearUsuarioComando comando = new CrearUsuarioComando
        (
            solicitud.Carnet,
            solicitud.Nombre,
            solicitud.ApellidoPaterno,
            solicitud.ApellidoMaterno,
            solicitud.Rol,
            solicitud.CarreraId,
            solicitud.Contrasena,
            solicitud.Email,
            solicitud.Telefono,
            solicitud.NombreReferencia,
            solicitud.TelefonoReferencia,
            solicitud.EmailReferencia
        );

        UsuarioResponseDto resultado = _crearUsuario.Handle(comando);
        UsuarioResponseDto respuesta = new UsuarioResponseDto
        {
            Carnet             = resultado.Carnet,
            Nombre             = resultado.Nombre,
            ApellidoPaterno    = resultado.ApellidoPaterno,
            ApellidoMaterno    = resultado.ApellidoMaterno,
            Rol                = resultado.Rol,
            CarreraId          = resultado.CarreraId,
            Email              = resultado.Email,
            Telefono           = resultado.Telefono,
            NombreReferencia   = resultado.NombreReferencia,
            TelefonoReferencia = resultado.TelefonoReferencia,
            EmailReferencia    = resultado.EmailReferencia,
            EstaEliminado      = resultado.EstaEliminado
        };

        return CreatedAtAction(nameof(ObtenerUsuarioPorCarnet), 
                               new { carnet = respuesta.Carnet }, respuesta);
    }

    [HttpGet("{carnet}")]
    public ActionResult<UsuarioResponseDto> ObtenerUsuarioPorCarnet(string carnet)
    {
        UsuarioResponseDto? resultado = _obtenerUsuario.Handle(new ObtenerUsuarioConsulta(carnet));
        if (resultado == null)
        {
            return NotFound();
        }

        UsuarioResponseDto respuesta = new UsuarioResponseDto
        {
            Carnet             = resultado.Carnet,
            Nombre             = resultado.Nombre,
            ApellidoPaterno    = resultado.ApellidoPaterno,
            ApellidoMaterno    = resultado.ApellidoMaterno,
            Rol                = resultado.Rol,
            CarreraId          = resultado.CarreraId,
            Email              = resultado.Email,
            Telefono           = resultado.Telefono,
            NombreReferencia   = resultado.NombreReferencia,
            TelefonoReferencia = resultado.TelefonoReferencia,
            EmailReferencia    = resultado.EmailReferencia,
            EstaEliminado      = resultado.EstaEliminado
        };

        return Ok(respuesta);
    }

    [HttpPut("{carnet}")]
    public IActionResult ActualizarUsuario(string carnet, [FromBody] UsuarioRequestDto solicitud)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ActualizarUsuarioComando comando = new ActualizarUsuarioComando
        (
            carnet,
            solicitud.Nombre,
            solicitud.ApellidoPaterno,
            solicitud.ApellidoMaterno,
            solicitud.Rol,
            solicitud.CarreraId,
            solicitud.Contrasena,
            solicitud.Email,
            solicitud.Telefono,
            solicitud.NombreReferencia,
            solicitud.TelefonoReferencia,
            solicitud.EmailReferencia
        );

        UsuarioResponseDto? resultado = _actualizarUsuario.Handle(comando);
        if (resultado == null)
        {
            return NotFound();
        }

        UsuarioResponseDto respuesta = new UsuarioResponseDto
        {
            Carnet             = resultado.Carnet,
            Nombre             = resultado.Nombre,
            ApellidoPaterno    = resultado.ApellidoPaterno,
            ApellidoMaterno    = resultado.ApellidoMaterno,
            Rol                = resultado.Rol,
            CarreraId          = resultado.CarreraId,
            Email              = resultado.Email,
            Telefono           = resultado.Telefono,
            NombreReferencia   = resultado.NombreReferencia,
            TelefonoReferencia = resultado.TelefonoReferencia,
            EmailReferencia    = resultado.EmailReferencia,
            EstaEliminado      = resultado.EstaEliminado
        };

        return Ok(respuesta);
    }

    [HttpDelete("{carnet}")]
    public IActionResult EliminarUsuario(string carnet)
    {
        bool exito = _eliminarUsuario.Handle(new EliminarUsuarioComando(carnet));
        if (!exito)
        {
            return NotFound();
        }

        return NoContent();
    }
}
