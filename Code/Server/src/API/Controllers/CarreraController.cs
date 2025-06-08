using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly ICrearCarreraComando      _crear;
    private readonly IObtenerCarreraConsulta   _obtener;
    private readonly IActualizarCarreraComando _actualizar;
    private readonly IEliminarCarreraComando   _eliminar;

    public CarreraController(ICrearCarreraComando crear, IObtenerCarreraConsulta obtener,
                            IActualizarCarreraComando actualizar, IEliminarCarreraComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CarreraRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos de la carrera son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre de la carrera es requerido");

            var comando = new CrearCarreraComando(dto.Nombre);
            _crear.Handle(comando);
            
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear carrera: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<CarreraDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener carreras: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] CarreraRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos de la carrera son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre de la carrera es requerido");

            var comando = new ActualizarCarreraComando(id, dto.Nombre);
            _actualizar.Handle(comando);
            
            return Ok("Carrera actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar carrera: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarCarreraComando(id);
            _eliminar.Handle(comando);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar carrera: {ex.Message}");
        }
    }
}