using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;
using IMT_Reservas.Server.Application.Services.Implementations;

[ApiController]
[Route("api/[controller]")]
public class CarritoController : ControllerBase
{
    private readonly CarritoService _servicio;
    public CarritoController(CarritoService servicio) => _servicio = servicio;

    [HttpGet("fechasNoDisponibles")]
    public IActionResult ObtenerFechasNoDisponibles([FromQuery] ObtenerFechasNoDisponiblesComando input)
    {
        return Ok(_servicio.ObtenerFechasNoDisponibles(input));
    }

    [HttpGet("disponibilidadEquipos")]
    public IActionResult ObtenerDisponibilidadEquiposPorFechasYGrupos([FromQuery] ObtenerDisponibilidadEquiposComando input)
    {
        return Ok(_servicio.ObtenerDisponibilidadEquiposPorFechasYGrupos(input));
    }
}
