using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class ComponenteController : ControllerBase
{
    private readonly IComponenteService _servicio;
    public ComponenteController(IComponenteService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<ComponenteDto> Crear([FromBody] CrearComponenteComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<ComponenteDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<ComponenteDto> Actualizar([FromBody] ActualizarComponenteComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<ComponenteDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarComponenteComando(id));
    }
}
