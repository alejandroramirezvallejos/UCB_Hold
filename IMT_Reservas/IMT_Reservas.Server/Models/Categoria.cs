public class Categoria
{
    private int    _id;
    private string _nombre;
    private string _estadoPrestamo;

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

    public string EstadoPrestamo
    {
        get => _estadoPrestamo;
        private set
        {
            Enum enumEstadoDeCategoria = Verificar.SiEstaEnEnum<EstadoDeCategoria>(value, "El estado de la categoria");
            _estadoPrestamo = enumEstadoDeCategoria.ToString();
        }
    }

    public Categoria(string nombre, string estadoPrestamo)
    {
        Nombre         = nombre;
        EstadoPrestamo = estadoPrestamo;
    }
}