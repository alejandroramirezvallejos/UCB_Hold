using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICrearUsuarioComando      _crear;
    private readonly IObtenerUsuarioConsulta   _obtener;
    private readonly IActualizarUsuarioComando _actualizar;
    private readonly IEliminarUsuarioComando   _eliminar;

    public UsuarioController(ICrearUsuarioComando crear, IObtenerUsuarioConsulta obtener,
                             IActualizarUsuarioComando actualizar, IEliminarUsuarioComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public IActionResult CrearUsuario([FromBody] UsuarioRequestDto solicitud)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CrearUsuarioComando comando = new CrearUsuarioComando(
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

        UsuarioResponseDto resultado = _crear.Handle(comando);

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

        return CreatedAtAction(
            nameof(ObtenerUsuarioPorCarnet),
            new 
            { 
                carnet = respuesta.Carnet 
            },
            respuesta
        );
    }

    [HttpGet("{carnet}")]
    public ActionResult<UsuarioResponseDto> ObtenerUsuarioPorCarnet(string carnet)
    {
        ObtenerUsuarioConsulta consulta = new ObtenerUsuarioConsulta(carnet);
        UsuarioResponseDto? resultado = _obtener.Handle(consulta);

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

        ActualizarUsuarioComando comando = new ActualizarUsuarioComando(
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

        UsuarioResponseDto? resultado = _actualizar.Handle(comando);
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
        EliminarUsuarioComando comando = new EliminarUsuarioComando(carnet);
        bool exito = _eliminar.Handle(comando);

        if (!exito)
        {
            return NotFound();
        }

        return NoContent();
    }
}
