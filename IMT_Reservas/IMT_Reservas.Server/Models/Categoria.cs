using System.ComponentModel;

public enum CategoriaEstado
{
    [Description("Disponible")]
    Disponible,
    [Description("Prestado")]
    Prestado,
    [Description("Retirado")]
    Retirado
}

public class Categoria
{
    private int    _id;
    private string _nombre;
    private string _estadoPrestamo;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de categoria debe ser un numero natural: '{value}'",
                          nameof(value));
            _id = value;
        }
    }

    public string Nombre
    {
        get => _nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre de categoria no puede estar vacio",
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public string EstadoPrestamo
    {
        get => _estadoPrestamo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El estado de categoria no puede estar vacio",
                          nameof(value));

            var cleaned = value.Trim().ToLowerInvariant();
            var names = Enum.GetNames(typeof(CategoriaEstado));
            foreach (var name in names)
            {
                if (string.Equals(name, cleaned, StringComparison.OrdinalIgnoreCase))
                {
                    _estadoPrestamo = cleaned;
                    return;
                }
            }

            throw new ArgumentException($"El estado de categoria es invalido: '{value}'",
                      nameof(value));
        }
    }

    public Categoria(int id, string nombre, string estadoPrestamo)
    {
        Id             = id;
        Nombre         = nombre;
        EstadoPrestamo = estadoPrestamo;
    }
}