public interface IUsuarioRepository
{
    void Insert(Usuario usuario);
    void Update(Usuario usuario);
    void DeleteByCarnet(string carnet);
    Usuario? GetByCarnet(string carnet);
}