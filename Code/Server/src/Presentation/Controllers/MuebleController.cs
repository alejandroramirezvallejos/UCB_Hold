using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Mueble;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
	private readonly MuebleService _service;

	public MuebleController(MuebleService service)
	{
		_service = service;
	}
}
