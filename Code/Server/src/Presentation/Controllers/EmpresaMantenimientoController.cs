using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly Service<EmpresaMantenimientoEntity, EmpresaMantenimientoRepository, EmpresaMantenimientoDto> _service;

    public EmpresaMantenimientoController(Service<EmpresaMantenimientoEntity, EmpresaMantenimientoRepository, EmpresaMantenimientoDto> service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<EmpresaMantenimientoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmpresaMantenimientoDto dto)
    {
        var entity = new EmpresaMantenimientoEntity
        {
            Nombre = dto.NombreEmpresa ?? string.Empty,
            NombreResponsable = dto.NombreResponsable,
            ApellidoResponsable = dto.ApellidoResponsable,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<EmpresaMantenimientoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmpresaMantenimientoDto dto)
    {
        var entity = new EmpresaMantenimientoEntity
        {
            Id = id,
            Nombre = dto.NombreEmpresa ?? string.Empty,
            NombreResponsable = dto.NombreResponsable,
            ApellidoResponsable = dto.ApellidoResponsable,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
