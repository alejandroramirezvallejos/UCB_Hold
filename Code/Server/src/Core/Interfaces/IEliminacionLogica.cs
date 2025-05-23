public interface IEliminacionLogica
{
    bool EstaEliminado { get; }
    public void Eliminar();
    public void Recuperar();
}