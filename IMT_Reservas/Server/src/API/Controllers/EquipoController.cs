using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly IExecuteQuery _consulta;

    public EquipoController(IExecuteQuery consulta)
    {
        _consulta = consulta;
    }

    [HttpGet("equipos")]
    public IActionResult Obtener15Equipos()
    {
        const string nombreSp = "sp_obtener_15_equipos";
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        DataTable tabla = _consulta.EjecutarSp(nombreSp, parametros);

        List<Dictionary<string, object?>> listaEquipos = new List<Dictionary<string, object?>>();
        foreach (DataRow fila in tabla.Rows)
        {
            var equipo = new Dictionary<string, object?>();
            foreach (DataColumn columna in tabla.Columns)
            {
                object? valor = fila[columna] == DBNull.Value ? null : fila[columna];
                equipo[columna.ColumnName] = valor;
            }
            listaEquipos.Add(equipo);
        }

        return Ok(listaEquipos);
    }

    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] string termino)
    {
        const string nombreSp = "sp_buscar_equipos_por_termino";
        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            { "termino_busqueda", (object)$"%{termino}%" }
        };

        DataTable tabla = _consulta.EjecutarSp(nombreSp, parametros);
        List<Dictionary<string, object?>> listaEquipos = MapearTablaALista(tabla);

        return Ok(listaEquipos);
    }

    private List<Dictionary<string, object?>> MapearTablaALista(DataTable tabla)
    {
        var lista = new List<Dictionary<string, object?>>();
        foreach (DataRow fila in tabla.Rows)
        {
            var dict = new Dictionary<string, object?>();
            foreach (DataColumn columna in tabla.Columns)
            {
                object? valor = fila[columna] == DBNull.Value ? null : fila[columna];
                dict[columna.ColumnName] = valor;
            }
            lista.Add(dict);
        }
        return lista;
    }
}
