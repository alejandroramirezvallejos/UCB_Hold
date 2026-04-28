using System.Data;
using Npgsql;

public class GrupoEquipoRepository :
    ICrearRepository<CrearGrupoEquipoComando>,
    IActualizarRepository<ActualizarGrupoEquipoComando>,
    IEliminarRepository<EliminarGrupoEquipoComando>,
    IObtenerTodosRepository<CrearGrupoEquipoComando, DataTable>,
    IObtenerPorIdRepository<int, CrearGrupoEquipoComando, DataTable?>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GrupoEquipoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public void Crear(int idCategoria, CrearGrupoEquipoComando comando)
    {
        const string sql = @"INSERT INTO public.grupos_equipos (nombre, modelo, marca, descripcion, id_categoria, url_data_sheet, url_imagen, estado_eliminado, cantidad, costo_promedio)
                             VALUES (@nombre, @modelo, @marca, @descripcion, @idCategoria, @urlDataSheet, @urlImagen, FALSE, 0, 0)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["idCategoria"] = idCategoria,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear grupo de equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear grupo de equipo: {ex.Message}", ex); }
    }

    public void Crear(CrearGrupoEquipoComando comando)
    {
        throw new InvalidOperationException("Use Crear(int idCategoria, CrearGrupoEquipoComando comando) en su lugar.");
    }

    public DataTable? ObtenerPorId(int id)
    {
        const string sql = @"SELECT ge.id_grupo_equipo, ge.nombre AS nombre_grupo_equipo, ge.modelo AS modelo_grupo_equipo,
            ge.marca AS marca_grupo_equipo, ge.descripcion AS descripcion_grupo_equipo,
            c.nombre AS nombre_categoria, ge.url_data_sheet AS url_data_sheet_grupo_equipo,
            ge.url_imagen AS url_imagen_grupo_equipo, ge.cantidad AS cantidad_grupo_equipo, ge.costo_promedio
            FROM public.grupos_equipos AS ge
            INNER JOIN public.categorias AS c ON ge.id_categoria = c.id_categoria
            WHERE ge.id_grupo_equipo = @id AND ge.estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        try {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            if (dt.Rows.Count == 0) return null;
            return dt;
        } catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener grupo de equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener grupo de equipo: {ex.Message}", ex); }
    }

    public DataTable ObtenerPorNombreYCategoria(string? nombre, string? categoria)
    {
        const string sql = @"SELECT ge.id_grupo_equipo, ge.nombre AS nombre_grupo_equipo, ge.modelo AS modelo_grupo_equipo,
            ge.marca AS marca_grupo_equipo, ge.descripcion AS descripcion_grupo_equipo,
            c.nombre AS nombre_categoria, ge.url_data_sheet AS url_data_sheet_grupo_equipo,
            ge.url_imagen AS url_imagen_grupo_equipo, ge.cantidad AS cantidad_grupo_equipo, ge.costo_promedio
            FROM public.grupos_equipos AS ge
            INNER JOIN public.categorias AS c ON ge.id_categoria = c.id_categoria
            WHERE ge.estado_eliminado = FALSE
            AND (@nombre::text IS NULL OR ge.nombre ILIKE '%' || @nombre::text || '%')
            AND (@categoria::text IS NULL OR c.nombre ILIKE '%' || @categoria::text || '%')";
        try {
            return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
            {
                ["nombre"] = nombre ?? (object)DBNull.Value,
                ["categoria"] = categoria ?? (object)DBNull.Value
            });
        } catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener grupos de equipos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener grupos de equipos: {ex.Message}", ex); }
    }

    public void Actualizar(int? idCategoria, ActualizarGrupoEquipoComando comando)
    {
        const string sql = @"UPDATE public.grupos_equipos SET
            nombre = COALESCE(@nombre, nombre),
            modelo = COALESCE(@modelo, modelo),
            marca = COALESCE(@marca, marca),
            descripcion = COALESCE(@descripcion, descripcion),
            id_categoria = COALESCE(@idCategoria, id_categoria),
            url_data_sheet = COALESCE(@urlDataSheet, url_data_sheet),
            url_imagen = COALESCE(@urlImagen, url_imagen)
            WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["idCategoria"] = idCategoria ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar grupo de equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar grupo de equipo: {ex.Message}", ex); }
    }

    public void Actualizar(ActualizarGrupoEquipoComando comando)
    {
        Actualizar(null, comando);
    }

    public void Eliminar(EliminarGrupoEquipoComando comando)
    {
        const string sql = @"UPDATE public.grupos_equipos SET estado_eliminado = TRUE WHERE id_grupo_equipo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar grupo de equipo: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar grupo de equipo: {ex.Message}", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT ge.id_grupo_equipo, ge.nombre AS nombre_grupo_equipo, ge.modelo AS modelo_grupo_equipo,
            ge.marca AS marca_grupo_equipo, ge.descripcion AS descripcion_grupo_equipo,
            c.nombre AS nombre_categoria, ge.url_data_sheet AS url_data_sheet_grupo_equipo,
            ge.url_imagen AS url_imagen_grupo_equipo, ge.cantidad AS cantidad_grupo_equipo, ge.costo_promedio
            FROM public.grupos_equipos AS ge
            INNER JOIN public.categorias AS c ON ge.id_categoria = c.id_categoria
            WHERE ge.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener grupos de equipos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener grupos de equipos: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares para la lógica de negocio en el servicio ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteDuplicadoPorNombreModeloMarca(string nombre, string modelo, string marca)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE nombre = @nombre AND modelo = @modelo AND marca = @marca AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["modelo"] = modelo, ["marca"] = marca };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteDuplicadoPorNombreModeloMarcaExcluyendoId(string nombre, string modelo, string marca, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE nombre = @nombre AND modelo = @modelo AND marca = @marca AND estado_eliminado = FALSE AND id_grupo_equipo <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["modelo"] = modelo, ["marca"] = marca, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerCategoriaIdPorNombre(string nombreCategoria)
    {
        const string sql = @"SELECT id_categoria FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreCategoria };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public void ActualizarCantidad(int idGrupoEquipo, int incremento)
    {
        const string sql = @"UPDATE public.grupos_equipos SET cantidad = GREATEST(0, COALESCE(cantidad, 0) + @incremento) WHERE id_grupo_equipo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = idGrupoEquipo, ["incremento"] = incremento };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }

    public void ActualizarCostoPromedio(int idGrupoEquipo)
    {
        const string sql = @"UPDATE public.grupos_equipos SET costo_promedio = COALESCE(
            (SELECT AVG(costo_referencia) FROM public.equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE AND costo_referencia IS NOT NULL), 0)
            WHERE id_grupo_equipo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = idGrupoEquipo };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}