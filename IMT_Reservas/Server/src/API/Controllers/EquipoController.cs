using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public EquiposController(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    [HttpGet]
    public IActionResult ObtenerEquipos([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        const string consulta = "SELECT * FROM public.obtener_grupos_equipos_por_nombre_y_categoria(@nombre_grupo_equipo_input, @categoria_input)";

        string? nombreFiltro = string.IsNullOrWhiteSpace(nombre) ? null : nombre;
        string? categoriaFiltro = string.IsNullOrWhiteSpace(categoria) ? null : categoria;

        var parametros = new Dictionary<string, object>
        {
            { "nombre_grupo_equipo_input", nombreFiltro ?? (object)DBNull.Value },
            { "categoria_input",       categoriaFiltro ?? (object)DBNull.Value }
        };

        DataTable tabla = _ejecutarConsulta.EjecutarFuncion(consulta, parametros);
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
                dict[columna.ColumnName] = fila[columna] == DBNull.Value ? null : fila[columna];
            }
            lista.Add(dict);
        }
        return lista;
    }
}