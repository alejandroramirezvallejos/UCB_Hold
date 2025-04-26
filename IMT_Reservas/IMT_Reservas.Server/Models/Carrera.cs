using System.ComponentModel;

public enum NombreCarrera
{
    [Description("Ingenieria Mecatronica")]
    IngenieriaMecatronica,
    [Description("Ingenieria de Software")]
    IngenieriaDeSoftware
}
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
            if (value < 0)
                throw new ArgumentException($"El ID de carrera debe ser un numero natural: '{value}'",
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

            var limpio = value.Trim();
            var nombres = Enum.GetNames(typeof(NombreCarrera));

            foreach (var nombreEnum in nombres)
            {
                if (nombreEnum.Equals(limpio, StringComparison.OrdinalIgnoreCase))
                {
                    _nombre = limpio;
                    return;
                }
            }

            throw new ArgumentException($"El nombre de carrera es invalido: '{value}'",
                      nameof(value));
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