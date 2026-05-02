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
}
