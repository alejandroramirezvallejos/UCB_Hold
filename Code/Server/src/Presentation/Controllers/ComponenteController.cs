using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Componente;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
	private readonly ComponenteService _service;

	public ComponenteController(ComponenteService service)
	{
		_service = service;
	}
}
