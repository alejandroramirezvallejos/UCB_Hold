using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

public class UsuarioController : Controller<UsuarioDto, UsuarioService, CrearUsuarioComando, ActualizarUsuarioComando, EliminarUsuarioComando>
{
    public UsuarioController(UsuarioService servicio) : base(servicio) { }

    [HttpGet("{carnet}")]
    public IActionResult ObtenerPorCarnet(string carnet)
    {
        var usuarios = Servicio.ObtenerTodos();
        var usuario = usuarios?.Value?.FirstOrDefault(u => u.Carnet == carnet);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost("iniciarSesion")]
    public IActionResult IniciarSesion([FromBody] IniciarSesionUsuarioConsulta consulta) =>
        Ok(Servicio.IniciarSesionUsuario(consulta));
}
