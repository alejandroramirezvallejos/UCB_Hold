using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
	private readonly EmpresaMantenimientoService _service;

	public EmpresaMantenimientoController(EmpresaMantenimientoService service)
	{
		_service = service;
	}
}
