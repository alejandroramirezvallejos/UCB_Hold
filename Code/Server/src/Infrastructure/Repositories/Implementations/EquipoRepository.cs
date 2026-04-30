using System.Data;
using Ardalis.Result;

public class EquipoRepository : IEquipoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public EquipoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<EquipoDto?> Crear(int idGrupoEquipo, int codigoImt, int? idGavetero, CrearEquipoComando comando)
    {
        const string sql = @"INSERT INTO public.equipos (id_grupo_equipo, codigo_imt, descripcion, numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo, procedencia, id_gavetero, estado_eliminado, codigo_ucb)
                             VALUES (@idGrupoEquipo, @codigoImt, @descripcion, @numeroSerial, @ubicacion, @costoReferencia, @tiempoMaximoPrestamo, @procedencia, @idGavetero, FALSE, @codigoUcb)";
        var parametros = new Dictionary<string, object?>
        {
            ["idGrupoEquipo"] = idGrupoEquipo,
            ["codigoImt"] = codigoImt,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"] = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["costoReferencia"] = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["procedencia"] = comando.Procedencia ?? (object)DBNull.Value,
            ["idGavetero"] = idGavetero ?? (object)DBNull.Value,
            ["codigoUcb"] = comando.CodigoUcb ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new EquipoDto { CodigoImt = codigoImt, Descripcion = comando.Descripcion };
        return Result<EquipoDto?>.Created(dto);
    }

    public Result<EquipoDto?> Crear(CrearEquipoComando comando)
        => Result<EquipoDto?>.Error("Use Crear(int idGrupoEquipo, int codigoImt, int? idGavetero, CrearEquipoComando comando)");

    public Result<EquipoDto?> Actualizar(int? idGrupoEquipo, int? idGavetero, ActualizarEquipoComando comando)
    {
        const string sql = @"UPDATE public.equipos SET
            id_grupo_equipo = COALESCE(@idGrupoEquipo, id_grupo_equipo),
            descripcion = COALESCE(@descripcion, descripcion),
            numero_serial = COALESCE(@numeroSerial, numero_serial),
            ubicacion = COALESCE(@ubicacion, ubicacion),
            costo_referencia = COALESCE(@costoReferencia, costo_referencia),
            tiempo_max_prestamo = COALESCE(@tiempoMaximoPrestamo, tiempo_max_prestamo),
            procedencia = COALESCE(@procedencia, procedencia),
            id_gavetero = COALESCE(@idGavetero, id_gavetero),
            estado_equipo = COALESCE(@estadoEquipo::estado_equipo, estado_equipo),
            codigo_ucb = COALESCE(@codigoUcb, codigo_ucb)
            WHERE id_equipo = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["idGrupoEquipo"] = idGrupoEquipo ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["numeroSerial"] = comando.NumeroSerial ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["costoReferencia"] = comando.CostoReferencia ?? (object)DBNull.Value,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo ?? (object)DBNull.Value,
            ["procedencia"] = comando.Procedencia ?? (object)DBNull.Value,
            ["idGavetero"] = idGavetero ?? (object)DBNull.Value,
            ["estadoEquipo"] = comando.EstadoEquipo ?? (object)DBNull.Value,
            ["codigoUcb"] = comando.CodigoUcb ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new EquipoDto { Id = comando.Id, Descripcion = comando.Descripcion };
        return Result<EquipoDto?>.Success(dto);
    }

    public Result<EquipoDto?> Actualizar(ActualizarEquipoComando comando)
        => Actualizar(null, null, comando);

    public Result<EquipoDto?> Eliminar(EliminarEquipoComando comando)
    {
        const string sql = @"UPDATE public.equipos SET estado_eliminado = TRUE WHERE id_equipo = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<EquipoDto?>.Success(new EquipoDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT e.id_equipo, ge.nombre AS nombre_grupo_equipo, ge.modelo AS modelo_equipo,
            ge.marca AS marca_equipo, e.codigo_imt AS codigo_imt_equipo, e.codigo_ucb AS codigo_ucb_equipo,
            e.descripcion AS descripcion_equipo, e.numero_serial AS numero_serial_equipo,
            e.ubicacion AS ubicacion_equipo, e.procedencia AS procedencia_equipo,
            e.tiempo_max_prestamo AS tiempo_max_prestamo_equipo, g.nombre AS nombre_gavetero_equipo,
            e.estado_equipo AS estado_equipo_equipo, e.costo_referencia AS costo_referencia_equipo
            FROM public.equipos AS e
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            LEFT JOIN public.gaveteros AS g ON e.id_gavetero = g.id_gavetero
            WHERE e.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.equipos WHERE id_equipo = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerGrupoEquipoIdPorNombreModeloMarca(string nombre, string modelo, string marca)
    {
        const string sql = @"SELECT id_grupo_equipo FROM public.grupos_equipos WHERE nombre = @nombre AND modelo = @modelo AND marca = @marca AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["modelo"] = modelo, ["marca"] = marca };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerGaveteroIdPorNombre(string nombreGavetero)
    {
        const string sql = @"SELECT id_gavetero FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreGavetero };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int GenerarCodigoImt(int idCategoria)
    {
        const string sql = @"SELECT COALESCE(MAX(e.codigo_imt % 10000000), 0) + 1
            FROM public.equipos AS e
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            WHERE ge.id_categoria = @idCategoria";
        var parametros = new Dictionary<string, object?> { ["idCategoria"] = idCategoria };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        var sufijo = Convert.ToInt32(dt.Rows[0][0]);
        return idCategoria * 10000000 + sufijo;
    }

    public int? ObtenerCategoriaIdPorGrupoEquipoId(int idGrupoEquipo)
    {
        const string sql = @"SELECT id_categoria FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["id"] = idGrupoEquipo };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerGrupoEquipoIdPorEquipoId(int idEquipo)
    {
        const string sql = @"SELECT id_grupo_equipo FROM public.equipos WHERE id_equipo = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?> { ["id"] = idEquipo };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerEquipoIdPorCodigoImt(int codigoImt)
    {
        const string sql = @"SELECT id_equipo FROM public.equipos WHERE codigo_imt = @codigoImt AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["codigoImt"] = codigoImt };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}