using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class GrupoEquipoController : ControllerBase
{
    private readonly GrupoEquipoService _servicio;
    public GrupoEquipoController(GrupoEquipoService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<GrupoEquipoDto?> Crear([FromBody] CrearGrupoEquipoComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<GrupoEquipoDto?>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var consulta = new ObtenerGrupoEquipoPorIdConsulta(id);
        var resultado = _servicio.ObtenerGrupoEquipoPorId(consulta);
        return Ok(resultado);
    }

    [HttpGet("buscar")]
    public IActionResult ObtenerPorNombreYCategoria([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        var consulta = new ObtenerGrupoEquipoPorNombreYCategoriaConsulta(nombre, categoria);
        var resultado = _servicio.ObtenerGrupoEquipoPorNombreYCategoria(consulta);
        return Ok(resultado);
    }

    [HttpPut]
    public Result<GrupoEquipoDto?> Actualizar([FromBody] ActualizarGrupoEquipoComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<GrupoEquipoDto?> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarGrupoEquipoComando(id));
    }
}
