public class Categoria : ICategoria, IEliminacionLogica
{
    private int    _id;
    private string _nombre        = string.Empty;
    private bool   _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID de la categoria");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre de la categoria");
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Categoria(string nombre)
    {
        Nombre = nombre;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}