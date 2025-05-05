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

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            { "nombre_grupo_equipo_input", nombreFiltro ?? (object)DBNull.Value },
            { "categoria_input",       categoriaFiltro ?? (object)DBNull.Value }
        };

        DataTable tabla = _ejecutarConsulta.EjecutarFuncion(consulta, parametros);
        List<Dictionary<string, object?>> listaEquipos = MapearTablaALista(tabla);

        return Ok(listaEquipos);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerEquipoPorId(int id)
    {
        const string consulta = @"
            SELECT
                id_equipo,
                id_grupo_equipo,
                codigo_imt,
                codigo_ucb,
                descripcion,
                estado_equipo,
                numero_serial,
                ubicacion,
                costo_referencia,
                tiempo_max_prestamo,
                procedencia,
                id_gavetero,
                estado_eliminado,
                fecha_ingreso_equipo
            FROM public.equipos
            WHERE id_equipo = @id_equipo_input;";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            { "id_equipo_input", id }
        };

        DataTable tabla = _ejecutarConsulta.EjecutarFuncion(consulta, parametros);

        if (tabla.Rows.Count == 0)
            return NotFound($"No se encontró un equipo con ID {id}");

        DataRow fila = tabla.Rows[0];

        EquipoDto equipo = new EquipoDto
        {
            Id                   = Convert.ToInt32(fila["id_equipo"]),
            GrupoEquipoId        = Convert.ToInt32(fila["id_grupo_equipo"]),
            CodigoImt            = fila["codigo_imt"]?.ToString() ?? string.Empty,
            CodigoUcb            = fila["codigo_ucb"] as string,
            Descripcion          = fila["descripcion"] as string,
            NumeroSerial         = fila["numero_serial"] as string,
            Ubicacion            = fila["ubicacion"] as string,
            CostoReferencia      = fila["costo_referencia"] == DBNull.Value
                                   ? null
                                   : Convert.ToDouble(fila["costo_referencia"]),
            TiempoMaximoPrestamo = fila["tiempo_max_prestamo"] == DBNull.Value
                                   ? null
                                   : Convert.ToInt32(fila["tiempo_max_prestamo"]),
            Procedencia          = fila["procedencia"] as string,
            GaveteroId           = fila["id_gavetero"] == DBNull.Value
                                   ? null
                                   : Convert.ToInt32(fila["id_gavetero"]),
            EstadoDisponibilidad = fila["estado_equipo"]?.ToString() ?? string.Empty,
            EstaEliminado        = Convert.ToBoolean(fila["estado_eliminado"]),
            FechaDeIngreso       = DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_ingreso_equipo"]))
        };

        return Ok(equipo);
    }

    private static List<Dictionary<string, object?>> MapearTablaALista(DataTable tabla)
    {
        List<Dictionary<string, object?>> lista = new List<Dictionary<string, object?>>();
        foreach (DataRow fila in tabla.Rows)
        {
            Dictionary<string, object?> dict = new Dictionary<string, object?>();
            foreach (DataColumn columna in tabla.Columns)
            {
                dict[columna.ColumnName] = fila[columna] == DBNull.Value ? null : fila[columna];
            }
            lista.Add(dict);
        }
        return lista;
    }
}
