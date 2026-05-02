using System.Data;
using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public abstract class Repository<TDto> : IRepository<TDto> where TDto : class
{
    protected readonly ExecuteQuery ExecuteQuery;

    protected Repository(ExecuteQuery executeQuery) => ExecuteQuery = executeQuery;

    public virtual Result<TDto> Create(Dictionary<string, object?> parameters)
    {
        var sql = CreateStatement();
        if (string.IsNullOrEmpty(sql))
            return Result<TDto>.Error("SQL Create command invalid");

        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count == 0
            ? Result<TDto>.Error("Failed to create record")
            : Result<TDto>.Created(MapRowToDto(dt.Rows[0]));
    }

    public virtual Result<TDto> Update(Dictionary<string, object?> parameters)
    {
        var sql = UpdateStatement();
        if (string.IsNullOrEmpty(sql))
            return Result<TDto>.Error("SQL Update command invalid");

        ExecuteQuery.EjecutarSpNR(sql, parameters);
        return Result<TDto>.Success(null);
    }

    public virtual Result<object> Delete(int id)
    {
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var sql = DeleteStatement();
        if (string.IsNullOrEmpty(sql))
            return Result<object>.Error("SQL Delete command invalid");

        ExecuteQuery.EjecutarSpNR(sql, parameters);
        return Result<object>.Success(null);
    }

    public virtual Result<TDto> Get(int id)
    {
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var sql = SelectById();
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count == 0
            ? Result<TDto>.NotFound()
            : Result<TDto>.Success(MapRowToDto(dt.Rows[0]));
    }

    public virtual Result<List<TDto>> GetAll(QueryFilter filter = null)
    {
        var sql = SelectAll();
        var dt = ExecuteQuery.EjecutarFuncion(sql, filter?.Filters ?? new Dictionary<string, object?>());
        return dt?.Rows.Count == 0
            ? Result<List<TDto>>.NotFound()
            : Result<List<TDto>>.Success(dt.Rows.Cast<DataRow>().Select(MapRowToDto).ToList());
    }

    public virtual bool Exists(int id)
    {
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var sql = SelectExists();
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count > 0;
    }

    protected abstract string CreateStatement();
    protected abstract string UpdateStatement();
    protected abstract string DeleteStatement();
    protected abstract string SelectAll();
    protected virtual string SelectById() => SelectAll() + " WHERE id = @id";
    protected virtual string SelectExists() => "SELECT 1 WHERE 1=0";
    protected abstract TDto MapRowToDto(DataRow row);
}
