using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Usuario;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
	private readonly UsuarioService _service;

	public UsuarioController(UsuarioService service)
	{
		_service = service;
	}
}
