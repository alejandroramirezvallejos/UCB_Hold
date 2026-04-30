using System.Data;
using Ardalis.Result;

public class CarreraRepository : ICarreraRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public CarreraRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<CarreraDto> Crear(CrearCarreraComando comando)
    {
        const string sql = @"INSERT INTO public.carreras (nombre, estado_eliminado) VALUES (@nombre, FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = comando.Nombre };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new CarreraDto { Nombre = comando.Nombre };
        return Result<CarreraDto>.Created(dto);
    }

    public Result<CarreraDto> Eliminar(EliminarCarreraComando comando)
    {
        if (!ExisteActivaPorId(comando.Id))
            return Result<CarreraDto>.NotFound("No se encontró el registro especificado");

        const string sql = @"UPDATE public.carreras SET estado_eliminado = TRUE WHERE id_carrera = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<CarreraDto>.Success(new CarreraDto { Id = comando.Id });
    }

    public Result<CarreraDto> Actualizar(ActualizarCarreraComando comando)
    {
        if (!ExisteActivaPorId(comando.Id))
            return Result<CarreraDto>.NotFound("No se encontró el registro especificado");

        const string sql = @"UPDATE public.carreras SET nombre = COALESCE(@nombre, nombre) WHERE id_carrera = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new CarreraDto { Id = comando.Id, Nombre = comando.Nombre };
        return Result<CarreraDto>.Success(dto);
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT c.id_carrera, c.nombre AS nombre_carrera FROM public.carreras AS c WHERE c.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public bool ExisteActivaPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.carreras WHERE id_carrera = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.carreras WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.carreras WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_carrera <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ReactivarEliminadaPorNombre(string nombre)
    {
        const string sql = @"UPDATE public.carreras SET estado_eliminado = FALSE WHERE nombre = @nombre AND estado_eliminado = TRUE";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var checkSql = @"SELECT EXISTS(SELECT 1 FROM public.carreras WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var dt = _ejecutarConsulta.EjecutarFuncion(checkSql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public void EliminarLogicamentePorId(int id)
    {
        const string sql = @"UPDATE public.carreras SET estado_eliminado = TRUE WHERE id_carrera = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}