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
        const string consulta = "SELECT * FROM public.obtener_grupo_equipo_especifico_por_id(@id_equipo_input)";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            { "id_equipo_input", id }
        };

        DataTable tablaResultado = _ejecutarConsulta.EjecutarFuncion(consulta, parametros);

        if (tablaResultado.Rows.Count == 0)
        {
            return NotFound($"No se encontro un equipo con ID {id}");
        }

        DataRow fila = tablaResultado.Rows[0];

        EquipoDto equipo = new EquipoDto
        {
            Id                   = Convert.ToInt32(fila["id"]),
            GrupoEquipoId        = Convert.ToInt32(fila["grupo_equipo_id"]),
            CodigoImt            = fila["codigo_imt"].ToString() ?? string.Empty,
            CodigoUcb            = fila["codigo_ucb"] as string,
            Descripcion          = fila["descripcion"] as string,
            EstadoEquipo         = fila["estado_equipo"].ToString() ?? string.Empty,
            NumeroSerial         = fila["numero_serial"] as string,
            Ubicacion            = fila["ubicacion"] as string,
            CostoReferencia      = fila["costo_referencia"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToDouble(fila["costo_referencia"]),
            TiempoMaximoPrestamo = fila["tiempo_maximo_prestamo"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToInt32(fila["tiempo_maximo_prestamo"]),
            Procedencia          = fila["procedencia"] as string,
            GaveteroId           = fila["gavetero_id"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToInt32(fila["gavetero_id"]),
            EstadoDisponibilidad = fila["estado_disponibilidad"].ToString() ?? string.Empty,
            EstaEliminado        = Convert.ToBoolean(fila["esta_eliminado"]),
            FechaDeIngreso       = DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_de_ingreso"]))
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
