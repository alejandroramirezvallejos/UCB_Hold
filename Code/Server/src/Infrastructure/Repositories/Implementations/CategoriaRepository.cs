using System;
using System.Collections.Generic;
using System.Data;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public CategoriaRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public CategoriaDto Crear(CrearCategoriaComando comando)
    {
        const string sql = @"
            INSERT INTO public.categorias
              (nombre, estado_eliminado)
            VALUES (@nombre, false)
            RETURNING id_categoria, nombre, estado_eliminado;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return MapearFila(dt.Rows[0]);
    }

    public CategoriaDto? ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias
             WHERE id_categoria = @id;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFila(dt.Rows[0]);
    }

    public List<CategoriaDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<CategoriaDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFila(row));
        return lista;
    }

    public CategoriaDto? Actualizar(ActualizarCategoriaComando comando)
    {
        const string sql = @"
            UPDATE public.categorias
               SET nombre = @nombre
             WHERE id_categoria = @id;

            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias
             WHERE id_categoria = @id;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]     = comando.Id,
            ["nombre"] = comando.Nombre
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFila(dt.Rows[0]);
    }

    public bool Eliminar(int id)
    {
        const string sql = @"
            UPDATE public.categorias
               SET estado_eliminado = true
             WHERE id_categoria = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
        {
            ["id"] = id
        });
        return true;
    }

    private static CategoriaDto MapearFila(DataRow fila)
    {
        return new CategoriaDto
        {
            Id            = Convert.ToInt32(fila["id_categoria"]),
            Nombre        = fila["nombre"].ToString()!,
            EstaEliminado = Convert.ToBoolean(fila["estado_eliminado"])
        };
    }
}