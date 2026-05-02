namespace IMT_Reservas.Server.Infrastructure.Repositories;

public interface IComentarioRepository
{
	Task<bool> ExisteActivoPorId(string id);
}
