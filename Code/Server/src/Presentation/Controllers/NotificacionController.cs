using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Notificacion;
using NotificacionEntity = IMT_Reservas.Server.Core.Entities.Notificacion;

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
	public async Task<IActionResult> Create([FromBody] NotificacionEntity entity)
	{
		var result = await _service.CreateAsync(entity);
		return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value) : BadRequest(result.Errors);
	}

	[HttpPut]
	public async Task<IActionResult> Update([FromBody] NotificacionEntity entity)
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
}
