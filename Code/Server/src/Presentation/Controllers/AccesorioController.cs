using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly Service<AccesorioEntity, AccesorioRepository, AccesorioDto> _service;
    private readonly AccesorioRepository _repository;

    public AccesorioController(
        Service<AccesorioEntity, AccesorioRepository, AccesorioDto> service,
        AccesorioRepository repository)
    {
        _service = service;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<AccesorioDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<AccesorioDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccesorioDto dto)
    {
        var equipoId = dto.IdEquipo;
        if (!equipoId.HasValue && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado) && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImt);

        if (!equipoId.HasValue || equipoId.Value <= 0)
        {
            return BadRequest(new Response<object>
            {
                Status = 400,
                Errors = new List<string> { "Equipo no encontrado" }
            });
        }

        var entity = new AccesorioEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            UrlDataSheet = dto.UrlDataSheet,
            IdEquipo = equipoId.Value
        };
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<AccesorioDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AccesorioDto dto)
    {
        var equipoId = dto.IdEquipo;
        if (!equipoId.HasValue && !string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado) && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImt))
            equipoId = await _repository.GetEquipoByCodigoImt(codigoImt);

        if (!equipoId.HasValue || equipoId.Value <= 0)
        {
            return BadRequest(new Response<object>
            {
                Status = 400,
                Errors = new List<string> { "Equipo no encontrado" }
            });
        }

        var entity = new AccesorioEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            UrlDataSheet = dto.UrlDataSheet,
            IdEquipo = equipoId.Value
        };
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<AccesorioDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
