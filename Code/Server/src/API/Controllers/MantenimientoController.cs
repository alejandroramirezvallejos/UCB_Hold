using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly ICrearMantenimientoComando    _crear;
    private readonly IObtenerMantenimientoConsulta _obtener;
    private readonly IEliminarMantenimientoComando _eliminar;

    public MantenimientoController(ICrearMantenimientoComando crear, IObtenerMantenimientoConsulta obtener,
                                  IEliminarMantenimientoComando eliminar)
    {
        _crear    = crear;
        _obtener  = obtener;
        _eliminar = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] MantenimientoRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del mantenimiento son requeridos");

            if (dto.FechaMantenimiento == default)
                return BadRequest("La fecha de mantenimiento es requerida");

            if (dto.FechaFinalDeMantenimiento == default)
                return BadRequest("La fecha final de mantenimiento es requerida");

            if (dto.FechaFinalDeMantenimiento < dto.FechaMantenimiento)
                return BadRequest("La fecha final debe ser posterior a la fecha de inicio");

            if (string.IsNullOrWhiteSpace(dto.NombreEmpresaMantenimiento))
                return BadRequest("El nombre de la empresa de mantenimiento es requerido");

            if (dto.CodigoIMT == null || dto.CodigoIMT.Length == 0)
                return BadRequest("Al menos un código IMT es requerido");

            if (dto.TipoMantenimiento == null || dto.TipoMantenimiento.Length == 0)
                return BadRequest("Al menos un tipo de mantenimiento es requerido");

            if (dto.CodigoIMT.Length != dto.TipoMantenimiento.Length)
                return BadRequest("El número de códigos IMT debe coincidir con el número de tipos de mantenimiento");

            if (dto.DescripcionEquipo != null && dto.DescripcionEquipo.Length > 0 && 
                dto.DescripcionEquipo.Length != dto.CodigoIMT.Length)
                return BadRequest("Si se proporcionan descripciones de equipo, debe haber una por cada código IMT");
            
            if (dto.CodigoIMT.Any(codigo => codigo <= 0))
                return BadRequest("Todos los códigos IMT deben ser números positivos");
            
            if (dto.Costo.HasValue && dto.Costo.Value < 0)
                return BadRequest("El costo debe ser un número positivo");

            var comando = new CrearMantenimientoComando(
                dto.FechaMantenimiento,
                dto.FechaFinalDeMantenimiento,
                dto.NombreEmpresaMantenimiento,
                dto.Costo,
                dto.DescripcionMantenimiento,
                dto.CodigoIMT,
                dto.TipoMantenimiento,
                dto.DescripcionEquipo
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear mantenimiento: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<MantenimientoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener mantenimientos: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarMantenimientoComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar mantenimiento: {ex.Message}");
        }
    }
}