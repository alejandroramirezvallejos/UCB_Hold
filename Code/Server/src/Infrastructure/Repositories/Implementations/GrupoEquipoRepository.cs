using System.Data;

public class GrupoEquipoRepository : IGrupoEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public GrupoEquipoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }
    public void Crear(CrearGrupoEquipoComando comando)
    {
        const string sql = @"
            CALL public.insertar_grupo_equipo(
	        @nombre,
	        @modelo,
	        @marca,
	        @descripcion,
	        @nombreCategoria,
	        @urlDataSheet,
	        @urlImagen
            )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["nombreCategoria"] = comando.NombreCategoria ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el grupo de equipo", ex);
        }
    }

    public GrupoEquipoDto? ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT * from public.obtener_grupo_equipo_especifico_por_id(
	        @id
            )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return MapearFilaADto(dt.Rows[0]);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el grupo de equipo por ID", ex);
        }
    }

    public List<GrupoEquipoDto> ObtenerPorNombreYCategoria(string? nombre, string? categoria)
    {
        const string sql = @"
            SELECT * from public.obtener_grupos_equipos_por_nombre_y_categoria(
	        @nombre,
	        @categoria    
            )";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
            {
                ["nombre"] = nombre ?? (object)DBNull.Value,
                ["categoria"] = categoria ?? (object)DBNull.Value
            });
            
            var lista = new List<GrupoEquipoDto>();
            foreach (DataRow fila in dt.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener grupos de equipos por nombre y categoría", ex);
        }
    }

    public void Actualizar(ActualizarGrupoEquipoComando comando)
    {
        const string sql = @"
        CALL public.actualizar_grupo_equipo(
	    @id,
	    @nombre,
	    @modelo,
	    @marca,
	    @descripcion,
	    @nombreCategoria,
	    @urlDataSheet,
	    @urlImagen
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["nombreCategoria"] = comando.NombreCategoria ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el grupo de equipo", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_grupo_equipo(
	        @id
            )";
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el grupo de equipo", ex);
        }
    }

    public List<GrupoEquipoDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_grupos_equipos()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<GrupoEquipoDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
    }

    private GrupoEquipoDto MapearFilaADto(DataRow fila)
    {
        return new GrupoEquipoDto
        {
            Id = Convert.ToInt32(fila["id_grupo_equipo"]),
            Nombre = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            Modelo = fila["modelo_grupo_equipo"] == DBNull.Value ? null : fila["modelo_grupo_equipo"].ToString(),
            Marca = fila["marca_grupo_equipo"] == DBNull.Value ? null : fila["marca_grupo_equipo"].ToString(),
            Descripcion = fila["descripcion_grupo_equipo"] == DBNull.Value ? null : fila["descripcion_grupo_equipo"].ToString(),
            NombreCategoria = fila["nombre_categoria"] == DBNull.Value ? null : fila["nombre_categoria"].ToString(),
            UrlDataSheet = fila["url_data_sheet_grupo_equipo"] == DBNull.Value ? null : fila["url_data_sheet_grupo_equipo"].ToString(),
            UrlImagen = fila["url_imagen_grupo_equipo"] == DBNull.Value ? null : fila["url_imagen_grupo_equipo"].ToString(),
            Cantidad = fila["cantidad_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["cantidad_grupo_equipo"])
        };
    }
}