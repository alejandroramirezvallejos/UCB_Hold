using System.Data;
using Ardalis.Result;

public class GrupoEquipoRepository : IGrupoEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GrupoEquipoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<GrupoEquipoDto?> Crear(int idCategoria, CrearGrupoEquipoComando comando)
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
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new GrupoEquipoDto { Nombre = comando.Nombre };
        return Result<GrupoEquipoDto?>.Created(dto);
    }

    public Result<GrupoEquipoDto?> Crear(CrearGrupoEquipoComando comando)
        => Result<GrupoEquipoDto?>.Error("Use Crear(int idCategoria, CrearGrupoEquipoComando comando)");

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
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : dt;
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
        return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["nombre"] = nombre ?? (object)DBNull.Value,
            ["categoria"] = categoria ?? (object)DBNull.Value
        });
    }

    public Result<GrupoEquipoDto?> Actualizar(int? idCategoria, ActualizarGrupoEquipoComando comando)
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
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new GrupoEquipoDto { Id = comando.Id, Nombre = comando.Nombre };
        return Result<GrupoEquipoDto?>.Success(dto);
    }

    public Result<GrupoEquipoDto?> Actualizar(ActualizarGrupoEquipoComando comando)
        => Actualizar(null, comando);

    public Result<GrupoEquipoDto?> Eliminar(EliminarGrupoEquipoComando comando)
    {
        const string sql = @"UPDATE public.grupos_equipos SET estado_eliminado = TRUE WHERE id_grupo_equipo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<GrupoEquipoDto?>.Success(new GrupoEquipoDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT ge.id_grupo_equipo, ge.nombre AS nombre_grupo_equipo, ge.modelo AS modelo_grupo_equipo,
            ge.marca AS marca_grupo_equipo, ge.descripcion AS descripcion_grupo_equipo,
            c.nombre AS nombre_categoria, ge.url_data_sheet AS url_data_sheet_grupo_equipo,
            ge.url_imagen AS url_imagen_grupo_equipo, ge.cantidad AS cantidad_grupo_equipo, ge.costo_promedio
            FROM public.grupos_equipos AS ge
            INNER JOIN public.categorias AS c ON ge.id_categoria = c.id_categoria
            WHERE ge.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

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
        return dt.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
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