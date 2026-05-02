using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Comentario.Dtos;
using IMT_Reservas.Server.Core.Abstractions;

namespace IMT_Reservas.Server.Infrastructure.Repositories;

public interface IComentarioRepository
{
	Task<bool> ExisteActivoPorId(string id);
	Task<Result<ComentarioListDto>> CreateAsync(Dictionary<string, object?> parameters);
	Task<Result<ComentarioListDto>> UpdateAsync(Dictionary<string, object?> parameters);
	Task<Result<List<ComentarioListDto>>> GetAllAsync(QueryFilter? filter = null);
	Task<Result<ComentarioListDto>> GetByIdAsync(int id);
	Task<Result<object>> DeleteAsync(int id);
}
