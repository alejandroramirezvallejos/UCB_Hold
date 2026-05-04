using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
using IMT_Reservas.Server.Application.Abstraction;
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

    [HttpPost("disponibilidadEquipos")]
    public async Task<IActionResult> DisponibilidadEquipos([FromBody] CarritoDto request)
    {
        var result = await _service.GetDisponibilidad(request);

        return result.IsSuccess
            ? Ok(new Response<List<CarritoDto>> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
