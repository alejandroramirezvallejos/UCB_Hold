using API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly ICrearPrestamoComando _crearPrestamoComando;
    private readonly IObtenerPrestamoConsulta _obtenerPrestamoConsulta;
    private readonly IEliminarPrestamoComando _eliminarPrestamoComando;

    public PrestamoController(
        ICrearPrestamoComando crearPrestamoComando,
        IObtenerPrestamoConsulta obtenerPrestamoConsulta,
        IEliminarPrestamoComando eliminarPrestamoComando)
    {
        _crearPrestamoComando = crearPrestamoComando;
        _obtenerPrestamoConsulta = obtenerPrestamoConsulta;
        _eliminarPrestamoComando = eliminarPrestamoComando;
    }

    [HttpPost]
    public IActionResult CrearPrestamo([FromBody] PrestamoRequestDto dto)
    {
        try
        {
            // Validaciones de ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validación de fechas
            if (dto.FechaPrestamoEsperada < DateTime.Today)
            {
                return BadRequest("La fecha de préstamo esperada no puede ser en el pasado");
            }

            if (dto.FechaDevolucionEsperada <= dto.FechaPrestamoEsperada)
            {
                return BadRequest("La fecha de devolución esperada debe ser posterior a la fecha de préstamo esperada");
            }

            // Validación de período máximo de préstamo
            var diasDiferencia = (dto.FechaDevolucionEsperada - dto.FechaPrestamoEsperada).Days;
            if (diasDiferencia > 365)
            {
                return BadRequest("El período de préstamo no puede exceder 1 año");
            }

            if (diasDiferencia < 1)
            {
                return BadRequest("El período mínimo de préstamo es de 1 día");
            }

            // Validación de grupos de equipo
            if (dto.GrupoEquipoId == null || dto.GrupoEquipoId.Length == 0)
            {
                return BadRequest("Se debe especificar al menos un grupo de equipo");
            }

            if (dto.GrupoEquipoId.Any(id => id <= 0))
            {
                return BadRequest("Todos los IDs de grupo de equipo deben ser números positivos");
            }

            // Validación de duplicados en grupos de equipo
            if (dto.GrupoEquipoId.Length != dto.GrupoEquipoId.Distinct().Count())
            {
                return BadRequest("No se pueden repetir grupos de equipo en el mismo préstamo");
            }

            // Validación de carnet
            if (string.IsNullOrWhiteSpace(dto.CarnetUsuario))
            {
                return BadRequest("El carnet de usuario es obligatorio");
            }

            // Validación de contrato (tamaño y formato)
            if (dto.Contrato != null)
            {
                if (dto.Contrato.Length > 10485760) // 10MB
                {
                    return BadRequest("El archivo de contrato no puede exceder los 10MB");
                }

                if (dto.Contrato.Length == 0)
                {
                    return BadRequest("El archivo de contrato no puede estar vacío");
                }
            }

            var comando = new CrearPrestamoComando(
                dto.GrupoEquipoId,
                dto.FechaPrestamoEsperada,
                dto.FechaDevolucionEsperada,
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

    [HttpGet("{id}")]
    public IActionResult ObtenerPrestamoPorId(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser un número positivo");
            }

            var prestamos = _obtenerPrestamoConsulta.Handle();
            var prestamo = prestamos?.FirstOrDefault(p => p.CarnetUsuario != null); // Ajustar según ID disponible en DTO
            
            if (prestamo == null)
            {
                return NotFound("Préstamo no encontrado");
            }

            return Ok(prestamo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}

