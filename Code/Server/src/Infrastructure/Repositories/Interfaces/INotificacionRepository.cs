using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Notificacion.Dtos;
using IMT_Reservas.Server.Core.Abstractions;

namespace IMT_Reservas.Server.Infrastructure.Repositories;

public interface INotificacionRepository
{
	Task<bool> ExisteActivoPorId(int id);
	Task<Result<NotificacionListDto>> CreateAsync(Dictionary<string, object?> parameters);
	Task<Result<NotificacionListDto>> UpdateAsync(Dictionary<string, object?> parameters);
	Task<Result<List<NotificacionListDto>>> GetAllAsync(QueryFilter? filter = null);
	Task<Result<NotificacionListDto>> GetByIdAsync(int id);
	Task<Result<object>> DeleteAsync(int id);
}
