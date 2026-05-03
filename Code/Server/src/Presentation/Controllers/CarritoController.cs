using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarritoController : ControllerBase
{
    private readonly CarritoService _service;

    public CarritoController(CarritoService service)
    {
        _service = service;
    }

    [HttpPost("fechasnoDisponibles")]
    public async Task<IActionResult> GetUnavailableDates([FromBody] ObtenerFechasNoDisponiblesRequest request)
    {
        var result = await _service.GetUnavailableDates(request.FechaInicio, request.FechaFin, request.Carrito);

        return result.IsSuccess ? Ok(new Response<List<FechaNoDisponibleResponse?>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost("disponibilidadEquipos")]
    public async Task<IActionResult> GetAvailability([FromBody] ObtenerDisponibilidadRequest request)
    {
        var result = await _service.GetAvailability(request.FechaInicio, request.FechaFin, request.ArrayIds);

        return result.IsSuccess ? Ok(new Response<List<DisponibilidadEquipoResponse?>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
