using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly EquipoService _service;

    public EquipoController(EquipoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        
        return result.IsSuccess ? Ok(new Response<List<EquipoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        
        return result.IsSuccess ? Ok(new Response<EquipoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipoDto dto)
    {
        var entity = DtoToEntity(dto);
        var result = await _service.Create(entity);
        
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<EquipoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipoDto dto)
    {
        var entity = DtoToEntity(dto);
        entity.Id = id;
        var result = await _service.Update(entity);
        
        return result.IsSuccess ? Ok(new Response<EquipoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    private static EquipoEntity DtoToEntity(EquipoDto dto) => new()
    {
        IdGrupoEquipo = dto.IdGrupoEquipo ?? 0,
        IdGavetero    = dto.IdGavetero,
        CodigoUcb     = dto.CodigoUcb,
        NumeroSerial  = dto.NumeroSerial,
        EstadoEquipo  = ParseEstado(dto.EstadoEquipo),
        Ubicacion     = dto.Ubicacion,
        CostoReferencia      = dto.CostoReferencia,
        Descripcion          = dto.Descripcion,
        TiempoMaximoPrestamo = dto.TiempoMaximoPrestamo,
        Procedencia          = dto.Procedencia,
        FechaIngresoEquipo   = dto.FechaIngresoEquipo.HasValue
            ? DateOnly.FromDateTime(dto.FechaIngresoEquipo.Value)
            : DateOnly.FromDateTime(DateTime.UtcNow)
    };

    private static EstadoEquipo ParseEstado(string? s) => s?.ToLowerInvariant() switch
    {
        "parcialmente_operativo" => EstadoEquipo.ParcialmenteOperativo,
        "inoperativo"            => EstadoEquipo.Inoperativo,
        _                        => EstadoEquipo.Operativo
    };
}
