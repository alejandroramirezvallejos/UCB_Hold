using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly EmpresaMantenimientoService servicio;
    public EmpresaMantenimientoController(EmpresaMantenimientoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearEmpresaMantenimientoComando input)
    {
        servicio.Crear(input); return Created();
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarEmpresaMantenimientoComando input)
    {
        servicio.Actualizar(input); return Ok(new { mensaje = "Empresa actualizada exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        servicio.Eliminar(new EliminarEmpresaMantenimientoComando(id)); return NoContent();
    }
}