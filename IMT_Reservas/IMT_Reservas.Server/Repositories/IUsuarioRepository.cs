public interface IUsuarioRepository
{
    Task InsertAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteByCarnetAsync(string carnet);
    Task<Usuario?> GetByCarnetAsync(string carnet);
}

