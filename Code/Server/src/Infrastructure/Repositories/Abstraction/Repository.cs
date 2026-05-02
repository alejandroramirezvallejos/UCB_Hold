using System.Data;
using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public abstract class Repository<TDto> : IRepository<TDto> where TDto : class
{
	protected readonly ExecuteQuery ExecuteQuery;

	protected Repository(ExecuteQuery executeQuery) => ExecuteQuery = executeQuery;

	public virtual async Task<Result<TDto>> CreateAsync(Dictionary<string, object?> parameters)
	{
		var sql = Create();
		if (string.IsNullOrEmpty(sql))
			return Result<TDto>.Error("SQL Create command invalid");

		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		if (dt?.Rows.Count == 0)
			return Result<TDto>.Error("Failed to create record");

		return Result<TDto>.Created(MapRowToDto(dt.Rows[0]));
	}

	public virtual async Task<Result<TDto>> UpdateAsync(Dictionary<string, object?> parameters)
	{
		var sql = Update();
		if (string.IsNullOrEmpty(sql))
			return Result<TDto>.Error("SQL Update command invalid");

		ExecuteQuery.EjecutarSpNR(sql, parameters);
		return Result<TDto>.Success(null);
	}

	public virtual async Task<Result<object>> DeleteAsync(int id)
	{
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var sql = Delete();
		if (string.IsNullOrEmpty(sql))
			return Result<object>.Error("SQL Delete command invalid");

		ExecuteQuery.EjecutarSpNR(sql, parameters);
		return Result<object>.Success(null);
	}

	public virtual async Task<Result<TDto>> GetByIdAsync(int id)
	{
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var sql = SelectById();
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		if (dt?.Rows.Count == 0)
			return Result<TDto>.NotFound();

		return Result<TDto>.Success(MapRowToDto(dt.Rows[0]));
	}

	public virtual async Task<Result<List<TDto>>> GetAllAsync(QueryFilter filter = null)
	{
		var sql = SelectAll();
		var dt = ExecuteQuery.EjecutarFuncion(sql, filter?.Filters ?? new Dictionary<string, object?>());
		if (dt?.Rows.Count == 0)
			return Result<List<TDto>>.NotFound();

		var dtos = dt.Rows.Cast<DataRow>().Select(MapRowToDto).ToList();
		return Result<List<TDto>>.Success(dtos);
	}

	public virtual async Task<bool> ExistsAsync(int id)
	{
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var sql = SelectExists();
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0;
	}

	protected abstract string Create();
	protected abstract string Update();
	protected abstract string Delete();
	protected abstract string SelectAll();
	protected virtual string SelectById() => SelectAll() + " WHERE id = @id";
	protected virtual string SelectExists() => "SELECT 1 WHERE 1=0";
	protected abstract TDto MapRowToDto(DataRow row);
}
