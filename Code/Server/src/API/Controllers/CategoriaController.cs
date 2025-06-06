using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICrearCategoriaComando      _crear;
    private readonly IObtenerCategoriaConsulta   _obtener;
    private readonly IObtenerCategoriaConsulta  _listar;
    private readonly IActualizarCategoriaComando _actualizar;
    private readonly IEliminarCategoriaComando   _eliminar;

    public CategoriaController(ICrearCategoriaComando crear, IObtenerCategoriaConsulta obtener,
                               IObtenerCategoriaConsulta listar, IActualizarCategoriaComando actualizar,
                               IEliminarCategoriaComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _listar     = listar;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult<CategoriaDto> Crear([FromBody] CategoriaRequestDto dto)
    {
        var comando = new CrearCategoriaComando(dto.Nombre);
        var resultado = _crear.Handle(comando);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id}")]
    public ActionResult<CategoriaDto> ObtenerPorId(int id)
    {
        var consulta = new ObtenerCategoriaConsulta(id);
        var dto = _obtener.Handle(consulta);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<List<CategoriaDto>> Listar(int Id)
    {
        var consulta = new ObtenerCategoriaConsulta(Id);
        return Ok(_listar.Handle(consulta));
    }

    [HttpPut("{id}")]
    public ActionResult<CategoriaDto> Actualizar(int id, [FromBody] CategoriaRequestDto dto)
    {
        var comando = new ActualizarCategoriaComando(id, dto.Nombre);
        var actualizado = _actualizar.Handle(comando);
        if (actualizado is null) return NotFound();
        return Ok(actualizado);
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        var comando = new EliminarCategoriaComando(id);
        _eliminar.Handle(comando);
        return NoContent();
    }
}
