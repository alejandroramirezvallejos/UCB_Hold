using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Mantenimiento;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
	private readonly MantenimientoService _service;

	public MantenimientoController(MantenimientoService service)
	{
		_service = service;
	}
}
