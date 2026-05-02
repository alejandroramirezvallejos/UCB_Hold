using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Categoria;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;

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
	public async Task<IActionResult> Create([FromBody] CategoriaEntity entity)
	{
		var result = await _service.CreateAsync(entity);
		return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value) : BadRequest(result.Errors);
	}

	[HttpPut]
	public async Task<IActionResult> Update([FromBody] CategoriaEntity entity)
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
