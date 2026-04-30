using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class CategoriaController : ControllerBase
{
    private readonly CategoriaService _servicio;
    public CategoriaController(CategoriaService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<CategoriaDto?> Crear([FromBody] CrearCategoriaComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<CategoriaDto?>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<CategoriaDto?> Actualizar([FromBody] ActualizarCategoriaComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<CategoriaDto?> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarCategoriaComando(id));
    }
}
