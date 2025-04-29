public class Carrera
{
    private int    _id;
    private string _nombre;
    private bool   _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID de la carrera");
    }

    public string Nombre
    {
        get => _nombre;
        private set
        {
            Enum enumVal = Verificar.SiEstaEnEnum<NombreDeCarrera>(value, "El nombre de la carrera");
            _nombre = enumVal.ToString();
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Carrera(string nombre)
    {
        Nombre = nombre;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
