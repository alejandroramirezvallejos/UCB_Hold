using System.Data;
using Ardalis.Result;

public class GaveteroRepository : IGaveteroRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GaveteroRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<GaveteroDto> Crear(int idMueble, CrearGaveteroComando comando)
    {
        const string sql = @"INSERT INTO public.gaveteros (nombre, tipo, id_mueble, longitud, profundidad, altura, estado_eliminado)
                             VALUES (@nombre, @tipo, @idMueble, @longitud, @profundidad, @altura, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idMueble"] = idMueble,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new GaveteroDto { Nombre = comando.Nombre, Tipo = comando.Tipo };
        return Result<GaveteroDto>.Created(dto);
    }

    public Result<GaveteroDto> Crear(CrearGaveteroComando comando)
        => Result<GaveteroDto>.Error("Use Crear(int idMueble, CrearGaveteroComando comando)");

    public Result<GaveteroDto> Actualizar(int? idMueble, ActualizarGaveteroComando comando)
    {
        const string sql = @"UPDATE public.gaveteros SET
            nombre = COALESCE(@nombre, nombre),
            tipo = COALESCE(@tipo, tipo),
            id_mueble = COALESCE(@idMueble, id_mueble),
            longitud = COALESCE(@longitud, longitud),
            profundidad = COALESCE(@profundidad, profundidad),
            altura = COALESCE(@altura, altura)
            WHERE id_gavetero = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idMueble"] = idMueble ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new GaveteroDto { Id = comando.Id, Nombre = comando.Nombre, Tipo = comando.Tipo };
        return Result<GaveteroDto>.Success(dto);
    }

    public Result<GaveteroDto> Actualizar(ActualizarGaveteroComando comando)
        => Actualizar(null, comando);

    public Result<GaveteroDto> Eliminar(EliminarGaveteroComando comando)
    {
        const string sql = @"UPDATE public.gaveteros SET estado_eliminado = TRUE WHERE id_gavetero = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<GaveteroDto>.Success(new GaveteroDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT g.id_gavetero, g.nombre AS nombre_gavetero, g.tipo AS tipo_gavetero,
            m.nombre AS nombre_mueble, g.longitud AS longitud_gavetero, g.profundidad AS profundidad_gavetero,
            g.altura AS altura_gavetero
            FROM public.gaveteros AS g
            INNER JOIN public.muebles AS m ON g.id_mueble = m.id_mueble
            WHERE g.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivoPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivoPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_gavetero <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerMuebleIdPorNombre(string nombreMueble)
    {
        const string sql = @"SELECT id_mueble FROM public.muebles WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreMueble };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerMuebleIdPorGaveteroId(int gaveteroId)
    {
        const string sql = @"SELECT id_mueble FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?> { ["id"] = gaveteroId };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
    }
}
