using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Categoria;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
	private readonly CategoriaService _service;

	public CategoriaController(CategoriaService service)
	{
		_service = service;
	}
}
