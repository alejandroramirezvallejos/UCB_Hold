using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Usuario;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;

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

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var result = await _service.GetAllAsync();
		return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		var result = await _service.GetByIdAsync(id);
		return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] UsuarioEntity entity)
	{
		var result = await _service.CreateAsync(entity);
		return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value) : BadRequest(result.Errors);
	}

	[HttpPut]
	public async Task<IActionResult> Update([FromBody] UsuarioEntity entity)
	{
		var result = await _service.UpdateAsync(entity);
		return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _service.DeleteAsync(id);
		return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
	}

	[HttpPost("iniciarSesion")]
	public async Task<IActionResult> IniciarSesion([FromBody] LoginRequest request)
	{
		if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Contrasena))
			return BadRequest(new { error = "Email y contraseña son requeridos" });

		var result = await _service.LoginAsync(request.Email, request.Contrasena);
		return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Errors);
	}
}

public class LoginRequest
{
	public string? Email { get; set; }
	public string? Contrasena { get; set; }
}
