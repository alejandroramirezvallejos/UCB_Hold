using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Prestamo;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
	private readonly PrestamoService _service;

	public PrestamoController(PrestamoService service)
	{
		_service = service;
	}
}
