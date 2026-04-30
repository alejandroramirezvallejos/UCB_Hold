using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class MantenimientoController : ControllerBase
{
    private readonly MantenimientoService _servicio;
    public MantenimientoController(MantenimientoService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<MantenimientoDto?> Crear([FromBody] CrearMantenimientoComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<MantenimientoDto?>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpDelete("{id}")]
    public Result<MantenimientoDto?> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarMantenimientoComando(id));
    }
}
