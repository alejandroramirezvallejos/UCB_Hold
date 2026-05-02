using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Accesorio;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
	private readonly AccesorioService _service;

	public AccesorioController(AccesorioService service)
	{
		_service = service;
	}
}
