using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Comentario;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentarioController : ControllerBase
{
	private readonly ComentarioService _service;

	public ComentarioController(ComentarioService service)
	{
		_service = service;
	}
}
