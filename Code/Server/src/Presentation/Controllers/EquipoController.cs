using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Equipo;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
	private readonly EquipoService _service;

	public EquipoController(EquipoService service)
	{
		_service = service;
	}
}
