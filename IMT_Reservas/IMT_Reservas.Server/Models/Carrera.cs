public class Carrera
{
    private int    _id;
    private string _nombre;
    private bool   _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de carrera debe ser un numero naturales: '{value}'",
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
                throw new ArgumentException("El nombre de carrera no puede estar vacio",
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Carrera(int id, string nombre)
    {
        Id            = id;
        Nombre        = nombre;
        EstaEliminado = false;
    }

    public void Eliminar() => EstaEliminado = true;
}