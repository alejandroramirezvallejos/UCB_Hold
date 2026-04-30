using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _servicio;
    public UsuarioController(IUsuarioService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<UsuarioDto> Crear([FromBody] CrearUsuarioComando comando)
    {
        return _servicio.Crear(comando);
    }

    [HttpGet]
    public Result<List<UsuarioDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<UsuarioDto> Actualizar([FromBody] ActualizarUsuarioComando comando)
    {
        return _servicio.Actualizar(comando);
    }

    [HttpDelete("{carnet}")]
    public Result<UsuarioDto> Eliminar(string carnet)
    {
        return _servicio.Eliminar(new EliminarUsuarioComando(carnet));
    }

    [HttpGet("{carnet}")]
    public IActionResult ObtenerPorCarnet(string carnet)
    {
        var usuarios = _servicio.ObtenerTodos();
        var usuario = usuarios?.Value?.FirstOrDefault(u => u.Carnet == carnet);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }

    [HttpPost("iniciarSesion")]
    public IActionResult IniciarSesion([FromBody] IniciarSesionUsuarioConsulta consulta)
    {
        var usuario = _servicio.IniciarSesionUsuario(consulta);
        return Ok(usuario);
    }
}
