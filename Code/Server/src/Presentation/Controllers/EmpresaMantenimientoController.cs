using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;

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
	public async Task<IActionResult> Create([FromBody] EmpresaMantenimientoEntity entity)
	{
		var result = await _service.CreateAsync(entity);
		return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value) : BadRequest(result.Errors);
	}

	[HttpPut]
	public async Task<IActionResult> Update([FromBody] EmpresaMantenimientoEntity entity)
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
