using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Abstraction;
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
    public async Task<IActionResult> GetUnavailableDates([FromBody] FechasNoDisponiblesRequest request)
    {
        var result = await _service.GetUnavailableDates(request.FechaInicio, request.FechaFin, request.Carrito);

        return result.IsSuccess ? Ok(new Response<List<FechasNoDisponiblesDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPost("disponibilidadEquipos")]
    public async Task<IActionResult> GetAvailability([FromBody] DisponibilidadRequest request)
    {
        var result = await _service.GetAvailability(request.FechaInicio, request.FechaFin, request.ArrayIds);

        return result.IsSuccess ? Ok(new Response<List<DisponibilidadDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
