using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IExecuteQuery _consulta;

    public EquiposController(IExecuteQuery consulta)
    {
        _consulta = consulta;
    }

    [HttpGet("equipos")]
    public IActionResult ObtenerEquipos([FromQuery] string grupoNombre, [FromQuery] string categoria)
    {
        if (string.IsNullOrWhiteSpace(grupoNombre) || string.IsNullOrWhiteSpace(categoria))
        {
            return BadRequest("Debe proporcionar el nombre o la categoria en los parámetros de consulta");
        }

        const string EjecutarConsulta = "SELECT * FROM public.obtener_grupos_equipos_por_nombre_y_categoria(@grupoNombre, @categoria)";

        var parametros = new Dictionary<string, object>
        {
            { "@grupoNombre", grupoNombre },
            { "@categoria", categoria }
        };

        DataTable tabla = _consulta.EjecutarFuncion(EjecutarConsulta, parametros);
        List<Dictionary<string, object?>> listaEquipos = MapearTablaALista(tabla);

        return Ok(listaEquipos);
    }

    private static List<Dictionary<string, object?>> MapearTablaALista(DataTable tabla)
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
