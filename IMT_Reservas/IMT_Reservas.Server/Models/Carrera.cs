public class Carrera
{
    public int    Id            { get; private set; }
    public string Nombre        { get; private set; }
    public bool   EstaEliminado { get; private set; }

    public Carrera(string nombre)
    {
        Nombre        = nombre;
        EstaEliminado = false;
    }
    public void Eliminar() => EstaEliminado = true;
}
