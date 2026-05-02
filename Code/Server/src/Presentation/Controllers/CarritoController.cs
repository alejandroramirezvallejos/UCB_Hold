using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrito;

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
	public IActionResult ObtenerFechasNoDisponibles([FromBody] dynamic request)
	{
		var result = _service.ObtenerFechasNoDisponibles(request.fechaInicio, request.fechaFin, request.carrito);
		return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
	}

	[HttpPost("disponibilidadEquipos")]
	public IActionResult ObtenerDisponibilidadEquipos([FromBody] dynamic request)
	{
		var result = _service.ObtenerDisponibilidadEquiposPorFechasYGrupos(request.fechaInicio, request.fechaFin, request.arrayIds);
		return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
	}
}
