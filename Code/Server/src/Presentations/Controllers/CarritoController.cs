using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.Services.Interfaces;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;

[ApiController]
[Route("api/[controller]")]
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _servicio;
    public CarritoController(ICarritoService servicio) => _servicio = servicio;

    [HttpGet("fechasNoDisponibles")]
    public IActionResult ObtenerFechasNoDisponibles([FromQuery] ObtenerFechasNoDisponiblesComando input)
    {
        try { return Ok(_servicio.ObtenerFechasNoDisponibles(input)); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("disponibilidadEquipos")]
    public IActionResult ObtenerDisponibilidadEquiposPorFechasYGrupos([FromQuery] ObtenerDisponibilidadEquiposComando input)
    {
        try { return Ok(_servicio.ObtenerDisponibilidadEquiposPorFechasYGrupos(input)); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}
