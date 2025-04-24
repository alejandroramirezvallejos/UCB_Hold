public enum CategoriaEstado { Disponible, Prestado, Retirado }

public class Categoria
{
    public int             Id             { get; private set; }
    public string          Nombre         { get; private set; }
    public CategoriaEstado EstadoPrestamo { get; private set; }

    public Categoria(string nombre, CategoriaEstado estadoPrestamo)
    {
        Nombre         = nombre;
        EstadoPrestamo = estadoPrestamo;
    }
}
