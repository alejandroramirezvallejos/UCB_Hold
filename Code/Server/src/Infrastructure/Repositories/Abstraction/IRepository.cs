using Ardalis.Result;

namespace IMT_Reservas.Server.Core.Abstractions;

public interface IRepository<TDto> where TDto : class
{
	Task<Result<TDto>> CreateAsync(Dictionary<string, object?> parameters);
	Task<Result<TDto>> UpdateAsync(Dictionary<string, object?> parameters);
	Task<Result<object>> DeleteAsync(int id);
	Task<Result<TDto>> GetByIdAsync(int id);
	Task<Result<List<TDto>>> GetAllAsync(QueryFilter filter = null);
	Task<bool> ExistsAsync(int id);
}
