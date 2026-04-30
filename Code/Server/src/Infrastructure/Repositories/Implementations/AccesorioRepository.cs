using System.Data;
using Ardalis.Result;

public class AccesorioRepository : IAccesorioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public AccesorioRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<AccesorioDto> Crear(int idEquipo, CrearAccesorioComando comando)
    {
        const string sql = @"INSERT INTO public.accesorios (nombre, descripcion, modelo, url_data_sheet, precio, id_equipo, tipo, estado_eliminado)
                             VALUES (@nombre, @descripcion, @modelo, @urlDataSheet, @precio, @idEquipo, @tipo, FALSE) RETURNING id_accesorio";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value
        };

        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new AccesorioDto
        {
            Nombre = comando.Nombre,
            Descripcion = comando.Descripcion,
            Modelo = comando.Modelo,
            Precio = comando.Precio ?? 0,
            Tipo = comando.Tipo
        };
        return Result<AccesorioDto>.Created(dto);
    }

    public Result<AccesorioDto> Crear(CrearAccesorioComando comando)
        => Result<AccesorioDto>.Error("Use Crear(int idEquipo, CrearAccesorioComando comando)");

    public Result<AccesorioDto> Actualizar(int? idEquipo, ActualizarAccesorioComando comando)
    {
        if (!ExisteActivoPorId(comando.Id))
            return Result<AccesorioDto>.NotFound("No se encontró el registro especificado");

        const string sql = @"UPDATE public.accesorios SET
            nombre = COALESCE(@nombre, nombre),
            descripcion = COALESCE(@descripcion, descripcion),
            modelo = COALESCE(@modelo, modelo),
            url_data_sheet = COALESCE(@urlDataSheet, url_data_sheet),
            precio = COALESCE(@precio, precio),
            id_equipo = COALESCE(@idEquipo, id_equipo),
            tipo = COALESCE(@tipo, tipo)
            WHERE id_accesorio = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value
        };

        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new AccesorioDto
        {
            Id = comando.Id,
            Nombre = comando.Nombre,
            Descripcion = comando.Descripcion,
            Modelo = comando.Modelo,
            Precio = comando.Precio ?? 0,
            Tipo = comando.Tipo
        };
        return Result<AccesorioDto>.Success(dto);
    }

    public Result<AccesorioDto> Actualizar(ActualizarAccesorioComando comando)
        => Actualizar(null, comando);

    public Result<AccesorioDto> Eliminar(EliminarAccesorioComando comando)
    {
        if (!ExisteActivoPorId(comando.Id))
            return Result<AccesorioDto>.NotFound("No se encontró el registro especificado");

        const string sql = @"UPDATE public.accesorios SET estado_eliminado = TRUE WHERE id_accesorio = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };

        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<AccesorioDto>.Success(new AccesorioDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT a.id_accesorio, a.nombre AS nombre_accesorio, a.descripcion AS descripcion_accesorio,
            a.modelo AS modelo_accesorio, a.url_data_sheet AS url_data_sheet_accesorio,
            a.precio AS precio_accesorio, a.tipo AS tipo_accesorio,
            e.codigo_imt AS codigo_imt_equipo
            FROM public.accesorios AS a
            INNER JOIN public.equipos AS e ON a.id_equipo = e.id_equipo
            WHERE a.estado_eliminado = FALSE";

        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.accesorios WHERE id_accesorio = @id AND estado_eliminado = FALSE)";
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