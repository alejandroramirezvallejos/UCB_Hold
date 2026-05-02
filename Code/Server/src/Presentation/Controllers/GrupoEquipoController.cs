using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
	private readonly GrupoEquipoService _service;

	public GrupoEquipoController(GrupoEquipoService service)
	{
		_service = service;
	}
}
