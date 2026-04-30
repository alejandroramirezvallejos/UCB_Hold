using System.Data;
using Ardalis.Result;

public class MuebleRepository : IMuebleRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MuebleRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<MuebleDto?> Crear(CrearMuebleComando comando)
    {
        const string sql = @"INSERT INTO public.muebles (nombre, tipo, costo, ubicacion, longitud, profundidad, altura, estado_eliminado)
                             VALUES (@nombre, @tipo, @costo, @ubicacion, @longitud, @profundidad, @altura, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new MuebleDto { Nombre = comando.Nombre, Tipo = comando.Tipo };
        return Result<MuebleDto?>.Created(dto);
    }

    public Result<MuebleDto?> Actualizar(ActualizarMuebleComando comando)
    {
        const string sql = @"UPDATE public.muebles SET
            nombre = COALESCE(@nombre, nombre),
            tipo = COALESCE(@tipo, tipo),
            costo = COALESCE(@costo, costo),
            ubicacion = COALESCE(@ubicacion, ubicacion),
            longitud = COALESCE(@longitud, longitud),
            profundidad = COALESCE(@profundidad, profundidad),
            altura = COALESCE(@altura, altura)
            WHERE id_mueble = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new MuebleDto { Id = comando.Id, Nombre = comando.Nombre, Tipo = comando.Tipo };
        return Result<MuebleDto?>.Success(dto);
    }

    public Result<MuebleDto?> Eliminar(EliminarMuebleComando comando)
    {
        const string sql = @"UPDATE public.muebles SET estado_eliminado = TRUE WHERE id_mueble = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<MuebleDto?>.Success(new MuebleDto { Id = comando.Id });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT m.id_mueble, m.nombre AS nombre_mueble, m.numero_gaveteros AS numero_gaveteros_mueble,
            m.ubicacion AS ubicacion_mueble, m.tipo AS tipo_mueble, m.costo AS costo_mueble,
            m.longitud AS longitud_mueble, m.profundidad AS profundidad_mueble, m.altura AS altura_mueble
            FROM public.muebles AS m WHERE m.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.muebles WHERE id_mueble = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public void ActualizarNumeroGaveteros(int idMueble, int incremento)
    {
        const string sql = @"UPDATE public.muebles SET numero_gaveteros = GREATEST(0, COALESCE(numero_gaveteros, 0) + @incremento) WHERE id_mueble = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = idMueble, ["incremento"] = incremento };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}