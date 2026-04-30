using System.Data;
using Ardalis.Result;

public class ComponenteRepository : IComponenteRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public ComponenteRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<ComponenteDto> Crear(int idEquipo, CrearComponenteComando comando)
    {
        const string sql = @"INSERT INTO public.componentes (nombre, modelo, tipo, id_equipo, descripcion, precio_referencia, url_data_sheet, estado_eliminado)
                             VALUES (@nombre, @modelo, @tipo, @idEquipo, @descripcion, @precioReferencia, @urlDataSheet, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new ComponenteDto
        {
            Nombre = comando.Nombre,
            Modelo = comando.Modelo,
            Tipo = comando.Tipo,
            Descripcion = comando.Descripcion,
            PrecioReferencia = comando.PrecioReferencia ?? 0
        };
        return Result<ComponenteDto>.Created(dto);
    }

    public Result<ComponenteDto> Crear(CrearComponenteComando comando)
        => Result<ComponenteDto>.Error("Use Crear(int idEquipo, CrearComponenteComando comando)");

    public Result<ComponenteDto> Actualizar(int? idEquipo, ActualizarComponenteComando comando)
    {
        const string sql = @"UPDATE public.componentes SET
            nombre = COALESCE(@nombre, nombre),
            modelo = COALESCE(@modelo, modelo),
            tipo = COALESCE(@tipo, tipo),
            id_equipo = COALESCE(@idEquipo, id_equipo),
            descripcion = COALESCE(@descripcion, descripcion),
            precio_referencia = COALESCE(@precioReferencia, precio_referencia),
            url_data_sheet = COALESCE(@urlDataSheet, url_data_sheet)
            WHERE id_componente = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new ComponenteDto
        {
            Id = comando.Id,
            Nombre = comando.Nombre,
            Modelo = comando.Modelo,
            Tipo = comando.Tipo,
            Descripcion = comando.Descripcion,
            PrecioReferencia = comando.PrecioReferencia ?? 0
        };
        return Result<ComponenteDto>.Success(dto);
    }

    public Result<ComponenteDto> Actualizar(ActualizarComponenteComando comando)
        => Actualizar(null, comando);

    public Result<ComponenteDto> Eliminar(EliminarComponenteComando comando)
    {
        const string sql = @"UPDATE public.componentes SET estado_eliminado = TRUE WHERE id_componente = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<ComponenteDto>.Success(new ComponenteDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT c.id_componente, c.nombre AS nombre_componente, c.modelo AS modelo_componente,
            c.tipo AS tipo_componente, c.descripcion AS descripcion_componente,
            c.precio_referencia AS precio_referencia_componente, c.url_data_sheet AS url_data_sheet_equipo,
            ge.nombre AS nombre_equipo, e.codigo_imt AS codigo_imt_equipo
            FROM public.componentes AS c
            INNER JOIN public.equipos AS e ON c.id_equipo = e.id_equipo
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            WHERE c.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.componentes WHERE id_componente = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerEquipoIdPorCodigoImt(int codigoImt)
    {
        const string sql = @"SELECT id_equipo FROM public.equipos WHERE codigo_imt = @codigoImt AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["codigoImt"] = codigoImt };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
    }
}
