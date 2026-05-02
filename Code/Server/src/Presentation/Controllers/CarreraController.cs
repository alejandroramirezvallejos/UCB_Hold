using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrera;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
	private readonly CarreraService _service;

	public CarreraController(CarreraService service)
	{
		_service = service;
	}
}
