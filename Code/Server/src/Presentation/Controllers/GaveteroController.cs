using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Gavetero;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
	private readonly GaveteroService _service;

	public GaveteroController(GaveteroService service)
	{
		_service = service;
	}
}
