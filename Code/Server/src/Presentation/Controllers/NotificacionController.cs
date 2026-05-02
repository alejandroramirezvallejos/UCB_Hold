using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Notificacion;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
	private readonly NotificacionService _service;

	public NotificacionController(NotificacionService service)
	{
		_service = service;
	}
}
