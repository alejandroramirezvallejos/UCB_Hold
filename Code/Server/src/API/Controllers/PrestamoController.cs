using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly ICrearPrestamoComando    _crearPrestamoComando;
    private readonly IObtenerPrestamoConsulta _obtenerPrestamoConsulta;
    private readonly IEliminarPrestamoComando _eliminarPrestamoComando;

    public PrestamoController(
        ICrearPrestamoComando crearPrestamoComando,
        IObtenerPrestamoConsulta obtenerPrestamoConsulta,
        IEliminarPrestamoComando eliminarPrestamoComando)
    {
        _crearPrestamoComando    = crearPrestamoComando;
        _obtenerPrestamoConsulta = obtenerPrestamoConsulta;
        _eliminarPrestamoComando = eliminarPrestamoComando;
    }

    [HttpPost]
    public IActionResult CrearPrestamo([FromBody] PrestamoRequestDto dto)
    {
        try
        {
            
            if (dto.GrupoEquipoId == null || dto.GrupoEquipoId.Length == 0)
            {
                return BadRequest("Se debe especificar al menos un grupo de equipo");
            }

            if (dto.GrupoEquipoId.Any(id => id <= 0))
            {
                return BadRequest("Todos los IDs de grupo de equipo deben ser números positivos");
            }
            
            if (string.IsNullOrWhiteSpace(dto.CarnetUsuario))
            {
                return BadRequest("El carnet de usuario es obligatorio");
            }
            if(dto.FechaPrestamoEsperada==null){
                return BadRequest("La fecha de préstamo esperada es obligatoria");
            }
            if(dto.FechaDevolucionEsperada==null){
                return BadRequest("La fecha de devolución esperada es obligatoria");
            }
            var comando = new CrearPrestamoComando(
                dto.GrupoEquipoId,
                (DateTime)dto.FechaPrestamoEsperada,
                (DateTime)dto.FechaDevolucionEsperada,
                dto.Observacion,
                dto.CarnetUsuario,
                dto.Contrato
            );

            _crearPrestamoComando.Handle(comando);
            return Ok(new { mensaje = "Préstamo creado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict($"Conflicto en la operación: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult ObtenerPrestamos()
    {
        try
        {
            var prestamos = _obtenerPrestamoConsulta.Handle();
            
            if (prestamos == null || !prestamos.Any())
            {
                return Ok(new List<PrestamoDto>());
            }

            return Ok(prestamos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult EliminarPrestamo(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo");
            }

            var comando = new EliminarPrestamoComando(id);
            _eliminarPrestamoComando.Handle(comando);
            
            return Ok(new { mensaje = "Préstamo eliminado exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Error de validación: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound($"Préstamo no encontrado: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}

