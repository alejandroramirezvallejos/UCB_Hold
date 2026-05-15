using IMT_Reservas.Server.Application.Features.Carrito;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class CarritoController : Controller
{
    private readonly CarritoService _service;

    public CarritoController(CarritoService service) => _service = service;

    [HttpPost("disponibilidadEquipos")]
    public async Task<IActionResult> DisponibilidadEquipos([FromBody] CarritoDto request)
        => ToResponse(await _service.GetDisponibilidad(request));
}
