using System.Data;
using Ardalis.Result;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ExecuteQuery ExecuteQuery;

    protected Repository(ExecuteQuery executeQuery) => ExecuteQuery = executeQuery;

    public virtual Result<TEntity?> Crear(dynamic comando)
    {
        var sql = GetInsertSql(comando);
        if (string.IsNullOrEmpty(sql))
            return Result<TEntity?>.Error("Comando inválido");

        var parametros = MapToParameters(comando);
        var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0
            ? Result<TEntity?>.Error("No se pudo crear el registro")
            : Result<TEntity?>.Created(MapRowToEntity(dt.Rows[0]));
    }

    public virtual Result<TEntity?> Actualizar(dynamic comando)
    {
        var sql = GetUpdateSql(comando);
        if (string.IsNullOrEmpty(sql))
            return Result<TEntity?>.Error("Comando inválido");

        var parametros = MapToParameters(comando);
        ExecuteQuery.EjecutarSpNR(sql, parametros);
        return Result<TEntity?>.Success(null);
    }

    public virtual Result<TEntity?> Eliminar(dynamic comando)
    {
        var sql = GetDeleteSql(comando);
        if (string.IsNullOrEmpty(sql))
            return Result<TEntity?>.Error("Comando inválido");

        var parametros = MapToParameters(comando);
        ExecuteQuery.EjecutarSpNR(sql, parametros);
        return Result<TEntity?>.Success(null);
    }

    public virtual Result<DataTable> ObtenerTodos()
    {
        var sql = GetSelectAllSql();
        var dt = ExecuteQuery.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró ningún registro")
            : Result<DataTable>.Success(dt);
    }

    protected abstract string GetInsertSql(dynamic comando);
    protected abstract string GetUpdateSql(dynamic comando);
    protected abstract string GetDeleteSql(dynamic comando);
    protected abstract string GetSelectAllSql();
    protected abstract Dictionary<string, object?> MapToParameters(dynamic comando);
    protected abstract TEntity? MapRowToEntity(DataRow row);
}
