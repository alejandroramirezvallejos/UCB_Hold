using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly ICrearGrupoEquipoComando      _crear;
    private readonly IObtenerGrupoEquipoConsulta   _obtener;
    private readonly IActualizarGrupoEquipoComando _actualizar;
    private readonly IEliminarGrupoEquipoComando   _eliminar;
    private readonly IObtenerGruposEquiposConsulta _lista;

    public GrupoEquipoController(ICrearGrupoEquipoComando crear, IObtenerGrupoEquipoConsulta obtener,
                                 IActualizarGrupoEquipoComando actualizar, IEliminarGrupoEquipoComando eliminar,
                                 IObtenerGruposEquiposConsulta lista)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
        _lista      = lista;
    }

    [HttpPost]
    public IActionResult Crear([FromBody] GrupoEquipoRequestDto solicitud)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var comando = new CrearGrupoEquipoComando(
            solicitud.Nombre,
            solicitud.Modelo,
            solicitud.UrlData,
            solicitud.UrlImagen,
            solicitud.Cantidad,
            solicitud.Marca,
            solicitud.CategoriaId
        );

        GrupoEquipoDto resultado = _crear.Handle(comando);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new 
            {
                id = resultado.Id
            },
            resultado
        );
    }

    [HttpGet("{id}")]
    public ActionResult<GrupoEquipoDto> ObtenerPorId(int id)
    {
        var consulta = new ObtenerGrupoEquipoConsulta(id);
        GrupoEquipoDto? resultado = _obtener.Handle(consulta);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    [HttpGet]
    public ActionResult<List<Dictionary<string, object?>>> ObtenerGruposEquipos(
        [FromQuery] string? nombre,
        [FromQuery] string? categoria)
    {
        try
        {
            throw new UsuarioNuloException("Error en la obtencion de articulos del buscador");
        }
        catch(UsuarioNuloException e)
        {
            Console.WriteLine(e.Message);
        }
        
        var consulta = new ObtenerGruposEquiposConsulta(nombre, categoria);
        var lista    = _lista.Handle(consulta);
        return Ok(lista);
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] GrupoEquipoRequestDto solicitud)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var comando = new ActualizarGrupoEquipoComando(
            id,
            solicitud.Nombre,
            solicitud.Modelo,
            solicitud.UrlData,
            solicitud.UrlImagen,
            solicitud.Cantidad,
            solicitud.Marca,
            solicitud.CategoriaId
        );

        GrupoEquipoDto? resultado = _actualizar.Handle(comando);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var comando = new EliminarGrupoEquipoComando(id);
        bool exito = _eliminar.Handle(comando);
        if (!exito) return NotFound();
        return NoContent();
    }
}
