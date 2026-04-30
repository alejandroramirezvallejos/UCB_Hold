using System.Data;
using Ardalis.Result;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public CategoriaRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<CategoriaDto?> Crear(CrearCategoriaComando comando)
    {
        const string sql = @"INSERT INTO public.categorias (nombre, estado_eliminado) VALUES (@nombre, FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = comando.Nombre };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new CategoriaDto { Nombre = comando.Nombre };
        return Result<CategoriaDto?>.Created(dto);
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT c.id_categoria, c.nombre AS categoria FROM public.categorias AS c WHERE c.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public Result<CategoriaDto?> Actualizar(ActualizarCategoriaComando comando)
    {
        const string sql = @"UPDATE public.categorias SET nombre = COALESCE(@nombre, nombre) WHERE id_categoria = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new CategoriaDto { Id = comando.Id, Nombre = comando.Nombre };
        return Result<CategoriaDto?>.Success(dto);
    }

    public Result<CategoriaDto?> Eliminar(EliminarCategoriaComando comando)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = TRUE WHERE id_categoria = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<CategoriaDto?>.Success(new CategoriaDto { Id = comando.Id });
    }

    public bool ExisteActivaPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE id_categoria = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_categoria <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ReactivarEliminadaPorNombre(string nombre)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = FALSE WHERE nombre = @nombre AND estado_eliminado = TRUE";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var checkSql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var dt = _ejecutarConsulta.EjecutarFuncion(checkSql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public void EliminarLogicamentePorId(int id)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = TRUE WHERE id_categoria = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}