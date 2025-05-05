using System;
using System.Collections.Generic;
using System.Data;

public class GrupoEquipoUseCase :ICrearGrupoEquipoComando, IObtenerGrupoEquipoConsulta,
                                 IObtenerGruposEquiposConsulta, IActualizarGrupoEquipoComando,
                                 IEliminarGrupoEquipoComando
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public GrupoEquipoUseCase(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public GrupoEquipoDto Handle(CrearGrupoEquipoComando comando)
    {
        const string sql = @"
            INSERT INTO public.grupo_equipos
              (nombre, modelo, urldata, urlimagen, cantidad, marca, categoriaid, estaeliminado)
            VALUES
              (@nombre, @modelo, @urldata, @urlimagen, @cantidad, @marca, @categoriaid, false)
            RETURNING
              id_grupo_equipo,
              nombre,
              modelo,
              urldata,
              urlimagen,
              cantidad,
              marca,
              categoriaid,
              estaeliminado;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["nombre"]      = comando.Nombre,
            ["modelo"]      = comando.Modelo,
            ["urldata"]     = comando.UrlData,
            ["urlimagen"]   = comando.UrlImagen,
            ["cantidad"]    = comando.Cantidad,
            ["marca"]       = comando.Marca,
            ["categoriaid"] = comando.CategoriaId
        });

        return MapearFilaADto(dt.Rows[0]);
    }

    public GrupoEquipoDto? Handle(ObtenerGrupoEquipoConsulta consulta)
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

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            ["id"] = consulta.Id
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);

        if (dt.Rows.Count == 0)
            return null;

        return MapearFilaADto(dt.Rows[0]);
    }

    public List<Dictionary<string, object?>> Handle(ObtenerGruposEquiposConsulta consulta)
    {
        const string sql = @"
            SELECT *
              FROM public.obtener_grupos_equipos_por_nombre_y_categoria(
                     @nombre_grupo_equipo_input,
                     @categoria_input
              );
        ";

        object nombreDb    = string.IsNullOrWhiteSpace(consulta.Nombre)    ? (object)DBNull.Value : consulta.Nombre!;
        object categoriaDb = string.IsNullOrWhiteSpace(consulta.Categoria) ? (object)DBNull.Value : consulta.Categoria!;

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object>
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
    }

    public GrupoEquipoDto? Handle(ActualizarGrupoEquipoComando comando)
    {
        const string sql = @"
            UPDATE public.grupo_equipos
            SET
                nombre      = @nombre,
                modelo      = @modelo,
                urldata     = @urldata,
                urlimagen   = @urlimagen,
                cantidad    = @cantidad,
                marca       = @marca,
                categoriaid = @categoriaid
            WHERE id_grupo_equipo = @id;
            SELECT
                id_grupo_equipo,
                nombre,
                modelo,
                urldata,
                urlimagen,
                cantidad,
                marca,
                categoriaid,
                estaeliminado
              FROM public.grupo_equipos
             WHERE id_grupo_equipo = @id;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["id"]           = comando.Id,
            ["nombre"]       = comando.Nombre,
            ["modelo"]       = comando.Modelo,
            ["urldata"]      = comando.UrlData,
            ["urlimagen"]    = comando.UrlImagen,
            ["cantidad"]     = comando.Cantidad,
            ["marca"]        = comando.Marca,
            ["categoriaid"]  = comando.CategoriaId
        });

        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }

    public bool Handle(EliminarGrupoEquipoComando comando)
    {
        const string sql = @"
            UPDATE public.grupo_equipos
            SET estaeliminado = true
            WHERE id_grupo_equipo = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
        {
            ["id"] = comando.Id
        });

        return true;
    }

    private static GrupoEquipoDto MapearFilaADto(DataRow fila)
    {
        return new GrupoEquipoDto
        {
            Id            = Convert.ToInt32(fila["id_grupo_equipo"]),
            Nombre        = fila["nombre"]?.ToString()      ?? string.Empty,
            Modelo        = fila["modelo"]?.ToString()      ?? string.Empty,
            UrlData       = fila["url_data_sheet"] as string,
            UrlImagen     = fila["url_imagen"]?.ToString(),
            Cantidad      = Convert.ToInt32(fila["cantidad"]),
            Marca         = fila["marca"]?.ToString()       ?? string.Empty,
            CategoriaId   = Convert.ToInt32(fila["id_categoria"]),
            EstaEliminado = Convert.ToBoolean(fila["estado_eliminado"]),
            Descripcion   = fila["descripcion"]?.ToString() ?? string.Empty
        };
    }
}
