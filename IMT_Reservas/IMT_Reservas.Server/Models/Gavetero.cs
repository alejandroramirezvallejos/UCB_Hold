public class Gavetero
{
    public int    Id            { get; private set; }
    public string Nombre        { get; private set; }
    public string Tipo          { get; private set; }
    public string Dimensiones   { get; private set; } //! Esta en formato "LxWxH"
    public bool   EstaEliminado { get; private set; }

    public Gavetero(string nombre, string tipo, string dimensiones)
    {
        Nombre        = nombre;
        Tipo          = tipo;
        Dimensiones   = dimensiones;
        EstaEliminado = false;
    }
}
