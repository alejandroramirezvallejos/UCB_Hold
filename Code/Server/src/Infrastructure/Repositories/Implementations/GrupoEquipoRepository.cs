using System;
using System.Collections.Generic;
using System.Data;

public class GrupoEquipoRepository : IGrupoEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public GrupoEquipoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }    public GrupoEquipoDto Crear(CrearGrupoEquipoComando comando)
    {
        const string sql = @"
            INSERT INTO public.grupos_equipos
              (nombre, modelo, url_data_sheet, url_imagen, cantidad, marca, id_categoria, estado_eliminado, descripcion)
            VALUES
              (@nombre, @modelo, @urldata, @urlimagen, @cantidad, @marca, @categoriaid, false, @descripcion)
            RETURNING
              id_grupo_equipo,
              nombre,
              modelo,
              url_data_sheet,
              url_imagen,
              cantidad,
              marca,
              id_categoria,
              estado_eliminado,
              descripcion;
        ";        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["nombre"]      = comando.Nombre,
            ["modelo"]      = comando.Modelo,
            ["urldata"]     = comando.UrlData ?? (object)DBNull.Value,
            ["urlimagen"]   = comando.UrlImagen,
            ["cantidad"]    = comando.Cantidad,
            ["marca"]       = comando.Marca,
            ["categoriaid"] = comando.CategoriaId,
            ["descripcion"] = (object)DBNull.Value
        });

        return MapearFilaADto(dt.Rows[0]);
    }

    public GrupoEquipoDto? ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT
                id_grupo_equipo,
                nombre,
                modelo,
                url_data_sheet,
                url_imagen,
                cantidad,
                marca,
                id_categoria,
                estado_eliminado,
                descripcion
            FROM public.grupos_equipos
            WHERE id_grupo_equipo = @id;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);

        if (dt.Rows.Count == 0)
            return null;

        return MapearFilaADto(dt.Rows[0]);
    }

    public List<Dictionary<string, object?>> ObtenerPorNombreYCategoria(string? nombre, string? categoria)
    {
        const string sql = @"
            SELECT *
              FROM public.obtener_grupos_equipos_por_nombre_y_categoria(
                     @nombre_grupo_equipo_input,
                     @categoria_input
              );
        ";

        object nombreDb    = string.IsNullOrWhiteSpace(nombre)    ? (object)DBNull.Value : nombre!;
        object categoriaDb = string.IsNullOrWhiteSpace(categoria) ? (object)DBNull.Value : categoria!;

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["nombre_grupo_equipo_input"] = nombreDb,
            ["categoria_input"]           = categoriaDb
        });

        var lista = new List<Dictionary<string, object?>>();
        foreach (DataRow row in dt.Rows)
        {
            var dict = new Dictionary<string, object?>();
            foreach (DataColumn col in dt.Columns)
                dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
            lista.Add(dict);
        }
        return lista;
    }    public GrupoEquipoDto? Actualizar(ActualizarGrupoEquipoComando comando)
    {
        const string sql = @"
            UPDATE public.grupos_equipos
            SET
                nombre = @nombre,
                modelo = @modelo,
                url_data_sheet = @urldata,
                url_imagen = @urlimagen,
                cantidad = @cantidad,
                marca = @marca,
                id_categoria = @categoriaid
            WHERE id_grupo_equipo = @id;
            
            SELECT
                id_grupo_equipo,
                nombre,
                modelo,
                url_data_sheet,
                url_imagen,
                cantidad,
                marca,
                id_categoria,
                estado_eliminado,
                descripcion
              FROM public.grupos_equipos
             WHERE id_grupo_equipo = @id;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()
        {
            ["id"]           = comando.Id,
            ["nombre"]       = comando.Nombre,
            ["modelo"]       = comando.Modelo,
            ["urldata"]      = comando.UrlData ?? (object)DBNull.Value,
            ["urlimagen"]    = comando.UrlImagen,
            ["cantidad"]     = comando.Cantidad,
            ["marca"]        = comando.Marca,
            ["categoriaid"]  = comando.CategoriaId
        });

        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }    public bool Eliminar(int id)
    {
        const string sql = @"
            UPDATE public.grupos_equipos
            SET estado_eliminado = true
            WHERE id_grupo_equipo = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
        {
            ["id"] = id
        });

        return true;
    }    private static GrupoEquipoDto MapearFilaADto(DataRow fila)
    {
        return new GrupoEquipoDto
        {
            Id            = Convert.ToInt32(fila["id_grupo_equipo"]),
            Nombre        = fila["nombre"]?.ToString() ?? string.Empty,
            Modelo        = fila["modelo"]?.ToString() ?? string.Empty,
            UrlData       = fila["url_data_sheet"] == DBNull.Value ? null : fila["url_data_sheet"].ToString(),
            UrlImagen     = fila["url_imagen"]?.ToString() ?? string.Empty,
            Cantidad      = Convert.ToInt32(fila["cantidad"]),
            Marca         = fila["marca"]?.ToString() ?? string.Empty,
            CategoriaId   = Convert.ToInt32(fila["id_categoria"]),
            EstaEliminado = Convert.ToBoolean(fila["estado_eliminado"]),
            Descripcion   = fila["descripcion"] == DBNull.Value ? string.Empty : fila["descripcion"].ToString()!
        };
    }
}