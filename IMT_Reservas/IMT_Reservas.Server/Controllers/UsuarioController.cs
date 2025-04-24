using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private UsuarioService _service;
    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    [HttpPost("{carnet}")]
    public async Task<IActionResult> ObtenerUsuarioPorCarnet(string carnet)
    {
        var usuario= await _service.ObtenerUsuarioPorCarnet(carnet);
        if (usuario == null) { 
            return NotFound("Usuario no encontrado");
        }
        return Ok(usuario);
    }
    [HttpGet("CerrarSesion")]
    public void Cerrar()
    {
    }

    [HttpPost("CrearCuenta")]
    public void NuevaCuenta()
    {
    }
}