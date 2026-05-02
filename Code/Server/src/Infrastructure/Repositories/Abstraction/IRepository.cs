using Ardalis.Result;
namespace IMT_Reservas.Server.Core.Abstractions;

public interface IRepository<TDto> where TDto : class
{
    Result<TDto> Create(Dictionary<string, object?> parameters);
    Result<TDto> Update(Dictionary<string, object?> parameters);
    Result<object> Delete(int id);
    Result<TDto> Get(int id);
    Result<List<TDto>> GetAll(QueryFilter? filter = null);
    bool Exists(int id);
}
