using System.Data;

public class CategoriaUseCase : ICrearCategoriaComando, IObtenerCategoriaConsulta,
                                IObtenerCategoriasConsulta, IActualizarCategoriaComando,
                                IEliminarCategoriaComando
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public CategoriaUseCase(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public CategoriaDto Handle(CrearCategoriaComando comando)
    {
        const string sql = @"
            INSERT INTO public.categorias
              (nombre, estado_eliminado)
            VALUES (@nombre, false)
            RETURNING id_categoria, nombre, estado_eliminado;
        ";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            ["nombre"] = comando.Nombre
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return MapearFila(dt.Rows[0]);
    }

    public CategoriaDto? Handle(ObtenerCategoriaConsulta consulta)
    {
        const string sql = @"
            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias
             WHERE id_categoria = @id;
        ";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            ["id"] = consulta.Id
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFila(dt.Rows[0]);
    }

    public List<CategoriaDto> Handle(ObtenerCategoriasConsulta consulta)
    {
        const string sql = @"
            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object>());
        var lista = new List<CategoriaDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFila(row));
        return lista;
    }

    public CategoriaDto? Handle(ActualizarCategoriaComando comando)
    {
        const string sql = @"
            UPDATE public.categorias
               SET nombre = @nombre
             WHERE id_categoria = @id;

            SELECT id_categoria, nombre, estado_eliminado
              FROM public.categorias
             WHERE id_categoria = @id;
        ";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            ["id"]     = comando.Id,
            ["nombre"] = comando.Nombre
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFila(dt.Rows[0]);
    }

    public bool Handle(EliminarCategoriaComando comando)
    {
        const string sql = @"
            UPDATE public.categorias
               SET estado_eliminado = true
             WHERE id_categoria = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object>
        {
            ["id"] = comando.Id
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
